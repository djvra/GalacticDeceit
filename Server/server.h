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
    void startGame();

private slots:
    void handleNewTcpConnection();
    void handleTcpData(QTcpSocket *socket);
    void handleUdpDatagrams();
    void sendPlayerId();
    void sendPlayerData();

signals:
    void initPlayers(QMap<int, ClientData> clients);
    void updatePlayers(QMap<int, ClientData> clients);

private:
    QTcpServer *tcpServer;
    QUdpSocket *udpSocket;
    QTimer *eventTimer;
    QTimer *updateTimer;
    QMap<int, ClientData> clients;
    bool isGameStarted;

    void processReceivedData(const QByteArray &data);
};

#endif // SERVER_H
