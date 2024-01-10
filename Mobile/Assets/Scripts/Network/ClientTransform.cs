using System;
using UnityEngine;

[Serializable]
public class ClientTransform
{
    public Vector3 position;
    public int color;
    public string name;

    public ClientTransform(int color, Vector3 position, string name)
    {
        this.color = color;
        this.position = position;
        this.name = name;
    }

}