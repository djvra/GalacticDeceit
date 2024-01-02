using System;
using UnityEngine;

[Serializable]
public class ClientTransform
{
    public Vector3 position;
    public int color;

    public ClientTransform(int color, Vector3 position)
    {
        this.color = color;
        this.position = position;
    }

}