using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Session
{
    public enum Hardness
    {
        Easy,
        Normal,
        Hard,
        Insane
    }
    public Hardness hardness = Hardness.Normal;

    public int stage = 1;

    public bool practice = false;
    public bool arenaPractice = false;
    public bool stagePractice = false;

    //cheats
    public bool infiniteLives = false;
    public bool infiniteContinues = false;
    public bool infiniteBombs = false;
    public bool invincible = false;
    public bool halfspeed = false;
    public bool doublespeed = false;

    public CraftData[] craftDatas = new CraftData[2];
}
