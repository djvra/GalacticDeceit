#ifndef SERVER_H
#define SERVER_H

#include <QWidget>
#include <QTcpServer>
#include <QTcpSocket>
#include <QUdpSocket>
#include <QNetworkInterface>
#include <QJsonParseError>
#include <QJsonObject>
#include <QTimer>
#include <utils.h>

class Server : public QWidget
{
    Q_OBJECT

public:
    explicit Server(QWidget *parent = nullptr);
    ~Server();
    QString getLocalIpAddress();
    void start(int port);
    void stop();

private slots:
    void handleNewTcpConnection();
    void handleTcpData(QTcpSocket *socket);
    void handleUdpDatagrams();
    void sendPlayerData();

/*
signals:
    void updatePlayer(PlayerTransform *ptransform);
*/

private:
    QTcpServer *tcpServer;
    QUdpSocket *udpSocket;
    QMap<int, ClientData> clients;

    void processReceivedData(const QByteArray &data);
};

#endif // SERVER_H
