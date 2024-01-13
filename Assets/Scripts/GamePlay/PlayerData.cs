using System;
using System.Collections;
using System.Collections.Generic;
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
}
