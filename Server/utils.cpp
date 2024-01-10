#include "utils.h"

QMap<Color, QString> Constants::colorToString = {
    {Red, "red"},
    {White, "white"},
    {Green, "green"},
    {Blue, "light-blue"},
    {Purple, "violet"},
    {Yellow, "yellow"}
};

PlayerTransform::PlayerTransform(float x, float y, bool isLive) :
    x(x), y(y), isLive(isLive)
{}

ClientData::ClientData(QString name, QHostAddress ip, int id, QTcpSocket *socket, Color skinColor)
    : name(name), ip(ip), id(id), playerTransform(0, 0, true), packetCounter(0), tcpSocket(socket), isImposter(false), skinColor(skinColor)
    , numRemainingTask(NUM_PLAYER_TASKS), alive(true)
{}

ClientData::ClientData()
    : name("username"), ip("0.0.0.0"), id(0), playerTransform(0, 0, true), packetCounter(0), isImposter(false), skinColor(Blue)
    , numRemainingTask(NUM_PLAYER_TASKS), alive(true)
{}
