#include "utils.h"

PlayerTransform::PlayerTransform(float x, float y, bool isLive) :
    x(x), y(y), isLive(isLive)
{
}

PlayerTransform::~PlayerTransform()
{
}

float PlayerTransform::getX() { return x; }

void PlayerTransform::setX(float x) { this->x = x; }

float PlayerTransform::getY() { return y; }


bool PlayerTransform::getLive() { return isLive; }

void PlayerTransform::setY(float y) {this->y = y; }

ClientData::ClientData(QString name, QHostAddress ip, int id, QTcpSocket *socket, Color skinColor)
    : name(name), ip(ip), id(id), playerTransform(0, 0, true), packetCounter(0), tcpSocket(socket), isImposter(false), skinColor(skinColor)
{}

ClientData::ClientData()
    : name("username"), ip("0.0.0.0"), id(0), playerTransform(0, 0, true), packetCounter(0), isImposter(false)
{}

ClientData::~ClientData() {}

QString ClientData::getName() { return name; }

void ClientData::setName(QString name) { this->name = name; }

int ClientData::getPacketCounter() { return packetCounter; }

void ClientData::setPacketCounter(int counter) { packetCounter = counter; }

QHostAddress ClientData::getIpAddress() { return ip; }

void ClientData::setIpAddress(QHostAddress ip) { this->ip = ip;}

PlayerTransform ClientData::getPlayerTransform() { return playerTransform; }

void ClientData::setPlayerTransform(PlayerTransform transform) { playerTransform = transform; }

QTcpSocket* ClientData::getQTcpSocket() { return tcpSocket; }

void ClientData::setQTcpSocket(QTcpSocket *socket) { tcpSocket = socket; }

int ClientData::getId() { return id; }

void ClientData::setId(int id) { this->id = id; }

bool ClientData::getImposter() { return isImposter; }

void ClientData::setImposter(bool isImposter) { this->isImposter = isImposter; }

Color ClientData::getSkinColor() { return skinColor; }

void ClientData::setSkinColor(Color skinColor) { this->skinColor = skinColor; }
