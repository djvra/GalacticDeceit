using System;
using UnityEngine;

[Serializable]
public class PlayerDataPacket
{
    public Vector3 position;
    public int clientId;
    public int packetCounter;

    public PlayerDataPacket(Vector3 position, int clientId, int packetCounter)
    {
        this.position = position;
        this.clientId = clientId;
        this.packetCounter = packetCounter;
    }
}