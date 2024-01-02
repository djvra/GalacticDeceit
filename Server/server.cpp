#include "server.h"

Server::Server(QWidget *parent)
    : QWidget(parent)
{
    tcpServer = new QTcpServer(this);
    udpSocket = new QUdpSocket(this);
    eventTimer = new QTimer(this);
    updateTimer = new QTimer(this);
    isGameStarted = false;

    connect(tcpServer, &QTcpServer::newConnection, this, &Server::handleNewTcpConnection);
    connect(udpSocket, &QUdpSocket::readyRead, this, &Server::handleUdpDatagrams);
}

Server::~Server()
{
}

void Server::start(int port)
{   
    if (!tcpServer->listen(QHostAddress::Any, port)) {
        qDebug() << "Server could not start!";
    } else {
        qDebug() << "Server started. Listening...";
    }

    if (!udpSocket->bind(QHostAddress::Any, Constants::SERVER_UDP_PORT)) {
        qDebug() << "UDP Socket could not bind!";
    } else {
        qDebug() << "UDP Socket bound to port" << Constants::SERVER_UDP_PORT;
    }
}

void Server::stop()
{
    // Stop the timers if they are running
    if (eventTimer && eventTimer->isActive()) {
        eventTimer->stop();
        delete eventTimer;
        eventTimer = nullptr;
    }

    if (updateTimer && updateTimer->isActive()) {
        updateTimer->stop();
        delete updateTimer;
        updateTimer = nullptr;
    }

    // Close TCP server
    if (tcpServer && tcpServer->isListening()) {
        tcpServer->close();
    }

    // Close UDP socket
    if (udpSocket && udpSocket->state() == QUdpSocket::BoundState) {
        udpSocket->close();
    }

    qDebug() << "Server stopped.";
}

void Server::startGame()
{
    chooseImposter();
    sendPlayerStartingInfo();

    isGameStarted = true;

    qDebug() << "Game is started.";

    // Set up a new timer to send the player informations on each frame
    if (eventTimer && eventTimer->isActive()) {
        eventTimer->stop();
    }

    connect(updateTimer, &QTimer::timeout, this, &Server::sendPlayerData);

    // Set the interval in milliseconds (e.g., 1000 ms = 1 second)
    int updateInterval = 100;
    updateTimer->start(updateInterval);
}

void Server::chooseImposter()
{
    // Select the imposter randomly
    int randomIndex = QRandomGenerator::global()->bounded(clients.size());
    clients[randomIndex].setImposter(true);
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
            //qDebug() << jsonStr;
            std::string stdStr = jsonStr.toStdString();
            qDebug() << stdStr.c_str();

            if (isGameStarted) {
                for (auto it = clients.begin(); it != clients.end(); ++it) {
                    ClientData data = it.value();
                    QTcpSocket* clientTcpSocket = data.getQTcpSocket();
                    QByteArray responseData = jsonStr.toUtf8();
                    clientTcpSocket->write(responseData); // Sending the response back to the client
                }
            }

            if (!isGameStarted) {
                QJsonObject jsonObj = jsonDoc.object();

                // Extract data from JSON object
                QString clientName = jsonObj["clientName"].toString();
                QString clientIp = jsonObj["clientIp"].toString();
                int clientId = clients.size();

                ClientData clientData(clientName, QHostAddress(clientIp), clientId, socket, (Color) clientId);
                clients.insert(clientId, clientData);

                qDebug() << "New client login from " << clientIp << ", with nickname" << clientName << ", given client id: " << clientId;
            }
        }
    }
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

        /*QString jsonStr = jsonDoc.toJson(QJsonDocument::Indented);
        //qDebug() << jsonStr;
        std::string stdStr = jsonStr.toStdString();
        qDebug() << stdStr.c_str();*/

        // Extract data from JSON object
        int clientId = jsonObj["clientId"].toInt();
        QJsonObject jsonPosition = jsonObj["position"].toObject();
        float x = jsonPosition.value("x").toDouble();
        float y = jsonPosition.value("y").toDouble();
        int packetCounter = jsonObj["packetCounter"].toInt();
        //qDebug() << "packetCounter: " << packetCounter;

        if (clients.contains(clientId)) {
            ClientData &data = clients[clientId];

            if (data.getPacketCounter() < packetCounter) {
                // Update client data fields as needed
                data.setPacketCounter(packetCounter);
                data.setPlayerTransform(PlayerTransform(x, y, true));
            }
        }
    }
}

void Server::sendPlayerStartingInfo()
{
    qDebug() << "Send player IDs and imposter information.";

    // Send serialized JSON data to each client
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        ClientData data = it.value();
        QTcpSocket* clientTcpSocket = data.getQTcpSocket();

        // Serialize client id and imposter information to JSON
        QJsonObject responseObj;
        responseObj["id"] = data.getId();
        responseObj["imposter"] = data.getImposter();
        responseObj["color"] = data.getSkinColor();

        QJsonDocument responseDoc(responseObj);
        QByteArray responseData = responseDoc.toJson(QJsonDocument::Compact);

        clientTcpSocket->write(responseData); // Sending the response back to the client
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
        PlayerTransform playerTransform = data.getPlayerTransform();
        QJsonObject positionObject;
        positionObject["x"] = playerTransform.getX();
        positionObject["y"] = playerTransform.getY();
        positionObject["z"] = 0;
        QJsonObject dataObject;
        dataObject["position"] = positionObject;
        dataObject["color"] = data.getSkinColor();
        clientsDataObject[QString::number(clientId)] = dataObject;
    }

    QJsonDocument doc(clientsDataObject);
    QByteArray payload = doc.toJson();

    // Send serialized JSON data to each client // ayni bilgisayarda test icin
    int portNumber = Constants::CLIENT_UDP_PORT;
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        QHostAddress clientIp = it.value().getIpAddress();
        //qDebug() << clientIp;
        //udpSocket->writeDatagram(payload, clientIp, Constants::CLIENT_UDP_PORT);

        // ayni bilgisayarda test etmek icin yazdim, normalde usttekini kullanacagiz
        udpSocket->writeDatagram(payload, clientIp, portNumber++);
    }

    emit updatePlayers(clients);
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
