#ifndef UTILS_H
#define UTILS_H

#include <QWidget>
#include <QNetworkInterface>
#include <QTcpSocket>
#include <QLabel>

#define NUM_PLAYER_TASKS 2

enum Color
{
    Red, White, Green, Blue, Purple, Yellow
};

enum ActionType
{
    Login, TaskDone, Killed, Report
};

class Constants {
public:
    static const int SERVER_TCP_PORT = 9000;
    static const int SERVER_UDP_PORT = 9001;
    static const int CLIENT_UDP_PORT = 9002;

    // Map enum values to icon file representations
    static QMap<Color, QString> colorToString;
};

class PlayerTransform
{
public:
    PlayerTransform(float x, float y, bool isLive);

    float x;
    float y;
    bool isLive;
};

class ClientData
{
public:
    ClientData(QString name, QHostAddress ip, int id, QTcpSocket *socket, Color skinColor);
    ClientData();

    QString name;
    QHostAddress ip;
    int id;
    PlayerTransform playerTransform;
    int packetCounter;
    QTcpSocket *tcpSocket;
    bool isImposter;
    Color skinColor;
    int numRemainingTask;
    bool alive;
};


struct PlayerLabels
{
    QLabel *name;
    QLabel *icon;
    QLabel *imposter;
    QLabel *alive;
};

#endif // UTILS_H
