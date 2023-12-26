#include "server.h"

Server::Server(QWidget *parent)
    : QWidget(parent)
{
    tcpServer = new QTcpServer(this);
    udpSocket = new QUdpSocket(this);

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
        // Handle error
    } else {
        qDebug() << "UDP Socket bound to port" << Constants::SERVER_UDP_PORT;
    }

    QTimer *updateTimer = new QTimer(this);
    connect(updateTimer, &QTimer::timeout, this, &Server::sendPlayerData);

    // Set the interval in milliseconds (e.g., 1000 ms = 1 second)
    int updateInterval = 100;
    updateTimer->start(updateInterval);
}

void Server::stop()
{

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
            QJsonObject jsonObj = jsonDoc.object();

            // Extract data from JSON object
            QString clientName = jsonObj["clientName"].toString();
            QString clientIp = jsonObj["clientIp"].toString();
            int clientId = clients.size();

            ClientData clientData(clientName, QHostAddress(clientIp), clientId);
            clients.insert(clientId, clientData);

            qDebug() << "New client login from " << clientIp << ", with nickname" << clientName << ", given client id: " << clientId;

            // Prepare and send the response JSON
            QJsonObject responseObj;
            responseObj["id"] = clientId; // Assuming you want to send the client ID as response

            QJsonDocument responseDoc(responseObj);
            QByteArray responseData = responseDoc.toJson(QJsonDocument::Compact);

            socket->write(responseData); // Sending the response back to the client

        }
    }
}

void Server::handleUdpDatagrams()
{
    while (udpSocket->hasPendingDatagrams()) {
        QByteArray datagram;
        datagram.resize(udpSocket->pendingDatagramSize());

        //QHostAddress senderAddress;
        //quint16 senderPort;
        //udpSocket->readDatagram(datagram.data(), datagram.size(), &senderAddress, &senderPort);
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
        QJsonObject clientTransform = jsonObj["clientTransform"].toObject();
        QJsonObject jsonPosition = clientTransform["position"].toObject();
        float x = jsonPosition.value("x").toDouble();
        float y = jsonPosition.value("y").toDouble();
        int packetCounter = jsonObj["packetCounter"].toInt();
        //qDebug() << "packetCounter: " << packetCounter;

        if (clients.contains(clientId)) {
            ClientData &data = clients[clientId];

            if (data.getPacketCounter() < packetCounter) {
                /*if(data.getPlayerTransform().getLive()){
                    qDebug() << "Got new data for user" << clientId << " X:"<<x<< " Y:"<<y;
                }*/

                // Update client data fields as needed
                data.setPacketCounter(packetCounter);
                // Todo: update player position
                data.setPlayerTransform(PlayerTransform(x, y, true));
            }
        }
    }
}

void Server::sendPlayerData()
{

    // Iterate through your clients
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        int clientId = it.key();
        ClientData &clientData = it.value();

        // Update player transforms or any other necessary data
        // PlayerTransform playerTransform = // Get player transform data

        // clientData.setPlayerTransform(playerTransform);
        // int packetCounter = // Get the updated packet counter
        // clientData.setPacketCounter(packetCounter);
    }

    // Serialize client transforms to JSON
    QJsonObject clientTransformsObject;
    for (auto it = clients.begin(); it != clients.end(); ++it) {
        int clientId = it.key();
        PlayerTransform playerTransform = it.value().getPlayerTransform();
        QJsonObject transformObject;
        QJsonObject positionObject;
        positionObject["x"] = playerTransform.getX();
        positionObject["y"] = playerTransform.getY();
        positionObject["z"] = 0;
        transformObject["position"] = positionObject;

        clientTransformsObject[QString::number(clientId)] = transformObject;
    }

    QJsonDocument doc(clientTransformsObject);
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
