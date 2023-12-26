#ifndef PLAYER_H
#define PLAYER_H

#include <string>

enum Color
{
    Red, White, Green, Blue, Purple, Yellow
};

class PlayerInfo
{
public:
    PlayerInfo(std::string pname, bool isImposter, Color skinColor);
    ~PlayerInfo();

private:
    std::string pname;
    bool isImposter;
    Color skinColor;
};

#endif // PLAYER_H
