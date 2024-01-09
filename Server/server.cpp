#include "server.h"

Server::Server(QWidget *parent)
    : QWidget(parent)
{
    tcpServer = new QTcpServer(this);
    udpSocket = new QUdpSocket(this);
    reportTimer = new QTimer(this);
    updateTimer = new QTimer(this);
    isGameStarted = false;
    numRemainingPlayers = 0;
    numRemaningVotes = 0;

    connect(tcpServer, &QTcpServer::newConnection, this, &Server::handleNewTcpConnection);
    connect(udpSocket, &QUdpSocket::readyRead, this, &Server::handleUdpDatagrams);

    connect(reportTimer, &QTimer::timeout, this, &Server::handleReport);
    connect(updateTimer, &QTimer::timeout, this, &Server::sendPlayerData);
}

Server::~Server()
{
}

bool Server::start(int port)
{
    if (!tcpServer->listen(QHostAddress::Any, port)) {
        qDebug() << "Server could not start!";
        return false;
    }

    qDebug() << "Server started. Listening...";

    if (!udpSocket->bind(QHostAddress::Any, Constants::SERVER_UDP_PORT)) {
        qDebug() << "UDP Socket could not bind!";
        return false;
    }

    qDebug() << "UDP Socket bound to port" << Constants::SERVER_UDP_PORT;
    return true;
}

void Server::stop()
{
    // Close TCP server
    if (tcpServer && tcpServer->isListening()) {
        tcpServer->close();
    }

    // Close UDP socket
    if (udpSocket && udpSocket->state() == QUdpSocket::BoundState) {
        udpSocket->close();
    }

    stopGame();

    qDebug() << "Server stopped.";
}

void Server::stopGame()
{
    // Stop the timers if they are running
    if (reportTimer->isActive())
        reportTimer->stop();

    if (updateTimer->isActive())
        updateTimer->stop();

    isGameStarted = false;
    clients.clear();
}

void Server::startGame()
{
    if (clients.size() < 1) {
        qDebug() << "Game couldn't started due to insufficient number of players.";
        return;
    }

    chooseImposter();
    sendPlayerStartingInfo();

    isGameStarted = true;
    numRemainingPlayers = clients.size();

    qDebug() << "Game is started.";

    // Set up a new timer to send the player informations on each frame
    if (updateTimer && updateTimer->isActive())
        updateTimer->stop();

    // Set the interval in milliseconds (e.g., 1000 ms = 1 second)
    int updateInterval = 100;
    updateTimer->start(updateInterval);
}

void Server::chooseImposter()
{
    // Select the imposter randomly
    int randomIndex = QRandomGenerator::global()->bounded(clients.size());
    clients[randomIndex].isImposter = true;
}

void Server::handleNewTcpConnection()
{
    QTcpSocket *clientSocket = tcpServer->nextPendingConnection();

    connect(clientSocket, &QTcpSocket::readyRead, this, [=]() {
        handleTcpData(clientSocket);
    });

    qDebug() << "New client connected..." ;
}

void Server::handleTcpData(QTcpSocket *socket)
{      
    if (socket->bytesAvailable() > 0) {
        QByteArray requestData = socket->readAll();
        QString jsonString(requestData);

        // Parse the JSON data
        QJsonParseError parseError;
        QJsonDocument jsonDoc = QJsonDocument::fromJson(jsonString.toUtf8(), &parseError);

        if (parseError.error != QJsonParseError::NoError) {
            qDebug() << "JSON parse error:" << parseError.errorString();
            return;
        }

        if (jsonDoc.isObject()) {
            QString jsonStr = jsonDoc.toJson(QJsonDocument::Indented);
            QJsonObject jsonObj = jsonDoc.object();
            QByteArray responseData = jsonStr.toUtf8();

            int actionType = jsonObj["actionType"].toInt();
            int id = jsonObj["id"].toInt();

            switch (actionType) {
            case Login:
                if (!isGameStarted) {
                    QJsonObject jsonObj = jsonDoc.object();
                    // Extract data from JSON object
                    QString clientName = jsonObj["clientName"].toString();
                    QString clientIp = jsonObj["clientIp"].toString();
                    int clientId = clients.size();

                    ClientData clientData(clientName, QHostAddress(clientIp), clientId, socket, (Color) clientId);
                    clients.insert(clientId, clientData);

                    qDebug() << "New client login from " << clientIp << ", with nickname" << clientName << ", given client id: " << clientId;
                    emit newLogin(clientData);
                }
                break;
            case Report:
                // Sending the response back to the client
                sendAllClients(responseData);
                numRemaningVotes = numRemainingPlayers;
                // Start timer for voting
                reportTimer->start(REPORT_TIMEOUT);
                break;
            case Vote:
                // TODO: Not implemented Yet
                if (collectedVotes.contains(id))
                    collectedVotes[id] = collectedVotes[id] + 1;
                else
                    collectedVotes[id] = 1;

                emit updateVotedPlayer(id, collectedVotes[id]);

                // Check if all the players have voted
                if (numRemaningVotes == 0)
                    handleReport();
                break;
            case Killed:
                qDebug() << "Killed player: " << id;
                clients[id].alive = false;
                --numRemainingPlayers;
                // Sending the response back to the client
                sendAllClients(responseData);
                checkGameStatus();
                // Notify the desktop application to be able to update the player status labels
                emit updatePlayer(clients[id]);
                break;
            case TaskDone:
                clients[id].numRemainingTask -= 1;
                checkGameStatus();
                break;
            default:
                break;
            }
        }
    }
}

void Server::handleReport()
{
    // Stop the timer in case all the players voted before the timeout
    if (reportTimer->isActive())
        reportTimer->stop();

    // Count the votes, select the winner
    if (collectedVotes.empty())
        return;


    int maxVotes = 0;
    QList<int> playersWithMaxVotes;

    // Iterate through the QMap to find the player(s) with the maximum number of votes
    for (auto it = collectedVotes.constBegin(); it != collectedVotes.constEnd(); ++it) {
        if (it.value() > maxVotes) {
            maxVotes = it.value();
            // Clear previous max-vote player(s)
            playersWithMaxVotes.clear();
            playersWithMaxVotes.append(it.key());
        } else if (it.value() == maxVotes) {
            // If there are multiple players with the same maximum votes, add them to the list
            playersWithMaxVotes.append(it.key());
        }
    }

    if (playersWithMaxVotes.size() == 1) {
        // Eleminate the player and send the kill signal to all the players
        QJsonObject jsonObj;
        jsonObj["actionType"] = Killed;
        jsonObj["id"] = playersWithMaxVotes[0];
        QJsonDocument jsonObjDoc(jsonObj);
        sendAllClients(jsonObjDoc.toJson(QJsonDocument::Compact));
    }

    collectedVotes.clear();

    emit resetVotes();
}

QMap<int, ClientData>::iterator Server::findImposter()
{
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        ClientData data = it.value();
        if (data.isImposter)
            return it; // Return the iterator if imposter is found
    }
    return clients.end(); // Return the end iterator if imposter is not found
}

void Server::checkGameStatus()
{
    // TODO: Revisite the finishing conditions
    if (isGameOver()) {
        QString winner;
        if (isImposterAlive()) {
            qDebug() << "Game is over. Imposter win!";
            winner = "imposter";
        }
        else {
            qDebug() << "Game is over. Crewmate win!";
            winner = "crewmates";
        }

        QJsonObject responseObj;
        responseObj["actionType"] = GameOver;
        responseObj["winner"] = winner;
        QJsonDocument responseDoc(responseObj);
        sendAllClients(responseDoc.toJson(QJsonDocument::Compact));

        // TODO-NEXT: Clear clients map, set isGameStarted as false
    }
}

bool Server::isImposterAlive()
{
    QMap<int, ClientData>::iterator imposter = findImposter();
    return imposter != clients.end() && imposter.value().alive;
}

bool Server::isGameOver()
{
    if (numRemainingPlayers == 1)
        return true;

    for (auto it = clients.begin(); it != clients.end(); ++it) {
        ClientData data = it.value();
        if (data.numRemainingTask > 0 && !data.isImposter)
            return false;
    }

    return true;
}

void Server::handleUdpDatagrams()
{
    while (udpSocket->hasPendingDatagrams()) {
        QByteArray datagram;
        datagram.resize(udpSocket->pendingDatagramSize());
        udpSocket->readDatagram(datagram.data(), datagram.size());
        processReceivedData(datagram);
    }
}

void Server::processReceivedData(const QByteArray &data)
{
    QJsonParseError parseError;
    QJsonDocument jsonDoc = QJsonDocument::fromJson(data, &parseError);

    if (parseError.error != QJsonParseError::NoError) {
        qDebug() << "JSON parse error:" << parseError.errorString();
        return;
    }

    if (jsonDoc.isObject()) {
        QJsonObject jsonObj = jsonDoc.object();

        // Extract data from JSON object
        int clientId = jsonObj["clientId"].toInt();
        QJsonObject jsonPosition = jsonObj["position"].toObject();
        float x = jsonPosition.value("x").toDouble();
        float y = jsonPosition.value("y").toDouble();
        int packetCounter = jsonObj["packetCounter"].toInt();

        if (clients.contains(clientId)) {
            ClientData &data = clients[clientId];

            if (data.packetCounter < packetCounter) {
                // Update client data fields as needed
                data.packetCounter = packetCounter;
                data.playerTransform = PlayerTransform(x, y, true);
            }
        }
    }
}

void Server::sendAllClients(QByteArray sendData)
{
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        QTcpSocket* clientTcpSocket = it.value().tcpSocket;
        clientTcpSocket->write(sendData);
    }
}

void Server::sendPlayerStartingInfo()
{
    // Send serialized JSON data to each client
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        ClientData data = it.value();
        QTcpSocket* clientTcpSocket = data.tcpSocket;

        // Serialize client id and imposter information to JSON
        QJsonObject responseObj;
        responseObj["actionType"] = GameStarted;
        responseObj["id"] = data.id;
        responseObj["imposter"] = data.isImposter;
        responseObj["color"] = data.skinColor;
        responseObj["numRemainingTask"] = data.numRemainingTask;

        QJsonDocument responseDoc(responseObj);
        QByteArray responseData = responseDoc.toJson(QJsonDocument::Compact);

        clientTcpSocket->write(responseData);
    }
    emit initPlayers(clients);
}

void Server::sendPlayerData()
{
    // Serialize client transforms to JSON
    QJsonObject clientsDataObject;
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        int clientId = it.key();
        ClientData data = it.value();
        PlayerTransform playerTransform = data.playerTransform;
        QJsonObject positionObject;
        positionObject["x"] = playerTransform.x;
        positionObject["y"] = playerTransform.y;
        positionObject["z"] = 0;
        QJsonObject dataObject;
        dataObject["position"] = positionObject;
        dataObject["color"] = data.skinColor;
        clientsDataObject[QString::number(clientId)] = dataObject;
    }

    QJsonDocument doc(clientsDataObject);
    QByteArray payload = doc.toJson();

    // Send serialized JSON data to each client // ayni bilgisayarda test icin
    // int portNumberTest = Constants::CLIENT_UDP_PORT;
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        QHostAddress clientIp = it.value().ip;

        udpSocket->writeDatagram(payload, clientIp, Constants::CLIENT_UDP_PORT);
        // ayni bilgisayarda test etmek icin yazdim, normalde usttekini kullanacagiz
        // udpSocket->writeDatagram(payload, clientIp, portNumberTest++);
    }

    emit updateGameMap(clients);
}

QString Server::getLocalIpAddress()
{
    QString localIpAddress;
    QList<QHostAddress> ipAddressesList = QNetworkInterface::allAddresses();
    // Retrieve the first non-localhost IPv4 address
    for (const QHostAddress &address : ipAddressesList) {
        if (address != QHostAddress::LocalHost && address.toIPv4Address()) {
            localIpAddress = address.toString();
            break;
        }
    }
    return localIpAddress;
}
