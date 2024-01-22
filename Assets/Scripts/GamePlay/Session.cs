using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public void Save(BinaryWriter writer)
    {
        craftDatas[0].Save(writer);
        if (GameManager.Instance.twoPlayer) craftDatas[1].Save(writer);
        writer.Write((int)hardness);
        writer.Write(stage);
    }
    public void Load(BinaryReader reader)
    {
        craftDatas[0].Load(reader);
        if (GameManager.Instance.twoPlayer) craftDatas[1].Load(reader);
        hardness = (Hardness)reader.ReadInt32();
        stage = reader.ReadInt32();
    }
}
