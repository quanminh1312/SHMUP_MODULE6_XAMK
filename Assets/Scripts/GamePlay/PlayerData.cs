using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public int score = 0;
    public int stageScore = 0;
    public byte lives = 3;

    public int chain = 0;
    public byte chainTimer = 0;
    public const int MAX_CHAIN = 200;

    //todo add other playthourgh stats

    public void Save(BinaryWriter writer)
    {
        writer.Write(score);
        writer.Write(lives);
    }
    public void Load(BinaryReader reader)
    {
        score = reader.ReadInt32();
        lives = reader.ReadByte();
    }
}
