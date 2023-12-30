using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int index;
}


[Serializable]
public struct BulletData
{
    public float positionX;
    public float positionY;
    public float dX;
    public float dY;
    public float angle;
    public int type;
    public bool active;
    public BulletData( float intX, float intY, float dX, float dY, float angle, int type, bool active)
    {
        this.positionX = intX;
        this.positionY = intY;
        this.dX = dX;
        this.dY = dY;
        this.angle = angle;
        this.type = type;
        this.active = active;
    }
}
