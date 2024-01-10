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
#include <QRandomGenerator>
#include <utils.h>

class Server : public QWidget
{
    Q_OBJECT

public:
    explicit Server(QWidget *parent = nullptr);
    ~Server();
    QString getLocalIpAddress();
    bool start(int port);
    void stop();
    void startGame();
    void stopGame();
    void chooseImposter();
    QMap<int, ClientData>::iterator findImposter();
    bool isImposterAlive();
    bool isGameOver();
    void checkGameStatus();
    void sendAllClients(QByteArray sendData);

private slots:
    void handleNewTcpConnection();
    void handleTcpData(QTcpSocket *socket);
    void handleUdpDatagrams();
    void sendPlayerStartingInfo();
    void sendPlayerData();
    void handleReport();

signals:
    void initPlayers(QMap<int, ClientData> clients);
    void updateGameMap(QMap<int, ClientData> clients);
    void updatePlayer(ClientData client);
    void updateVotedPlayer(int id, int numVotes);
    void resetVotes();
    void newLogin(ClientData client);

private:
    QTcpServer *tcpServer;
    QUdpSocket *udpSocket;
    QTimer *reportTimer;
    QTimer *updateTimer;
    QMap<int, ClientData> clients;
    QMap<int, int> collectedVotes;
    bool isGameStarted;
    bool isAllTasksDone;
    int numRemaningVotes;
    int numRemainingPlayers;

    void processReceivedData(const QByteArray &data);
};

#endif // SERVER_H
