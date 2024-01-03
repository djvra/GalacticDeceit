#ifndef UTILS_H
#define UTILS_H

#include <QWidget>
#include <QNetworkInterface>
#include <QTcpSocket>
#include <QLabel>

enum Color
{
    Red, White, Green, Blue, Purple, Yellow
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
    ~PlayerTransform();

    float getX();
    void setX(float x);

    float getY();
    void setY(float y);

    bool getLive();

private:
    float x;
    float y;
    bool isLive;
};

class ClientData
{
public:
    ClientData(QString name, QHostAddress ip, int id, QTcpSocket *socket, Color skinColor);
    ClientData();
    ~ClientData();

    QString getName();
    void setName(QString name);

    int getPacketCounter();
    void setPacketCounter(int counter);

    QHostAddress getIpAddress();
    void setIpAddress(QHostAddress ip);

    PlayerTransform getPlayerTransform();
    void setPlayerTransform(PlayerTransform transform);

    QTcpSocket* getQTcpSocket();
    void setQTcpSocket(QTcpSocket *socket);

    int getId();
    void setId(int id);

    bool getImposter();
    void setImposter(bool isImposter);

    Color getSkinColor();
    void setSkinColor(Color skinColor);

private:
    QString name;
    QHostAddress ip;
    int id;
    PlayerTransform playerTransform;
    int packetCounter;
    QTcpSocket *tcpSocket;
    bool isImposter;
    Color skinColor;
};

struct LoginRequest
{
    QString clientName;
    QString clientIp;

    LoginRequest(const QString& name) : clientName(name), clientIp("123") {}
};

struct PlayerLabels
{
    QLabel *name;
    QLabel *icon;
    QLabel *imposter;
};

#endif // UTILS_H
