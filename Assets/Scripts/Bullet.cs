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
    public float dAngle;    
    public int type;
    public bool active;
    public bool homing;
    public BulletData( float intX, float intY, float dX, float dY, float angle,float dAngle, int type, bool active, bool homing)
    {
        this.positionX = intX;
        this.positionY = intY;
        this.dX = dX;
        this.dY = dY;
        this.angle = angle;
        this.type = type;
        this.active = active;
        this.homing = homing;
        this.dAngle = dAngle;
    }
}
