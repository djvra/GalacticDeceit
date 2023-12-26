#ifndef UTILS_H
#define UTILS_H

#include <QWidget>
#include <QNetworkInterface>

class Constants {
public:
    static const int SERVER_TCP_PORT = 9000;
    static const int SERVER_UDP_PORT = 9001;
    static const int CLIENT_UDP_PORT = 9002;
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
    ClientData(QString name, QHostAddress ip, int id);
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

private:
    QString name;
    QHostAddress ip;
    int id;
    PlayerTransform playerTransform;
    int packetCounter;
};

struct LoginRequest
{
    QString clientName;
    QString clientIp;

    LoginRequest(const QString& name) : clientName(name), clientIp("123") {}
};
#endif // UTILS_H
