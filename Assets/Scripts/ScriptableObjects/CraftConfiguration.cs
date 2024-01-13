using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CraftConfig",menuName ="SHMUP/CraftConfiguration")]
public class CraftConfiguration : ScriptableObject
{
    public const int MAX_SHOT_POWER = 8;
    public const int MAX_SPEED = 10;
    public const int MAX_OPTION_POWER = 10;
    public const int MAX_BEAM_POWER = 10;
    public const int MAX_BOMB_POWER = 10;
    public Sprite craftSprite;

    public float speed;
    public float bulletStrength;
    public float beamPower;
    public byte bombPower;
    public byte optionPower;

    public ShotConfiguration[] shotlevel = new ShotConfiguration[MAX_SHOT_POWER];
}

[Serializable]
public class ShotConfiguration
{
    public int[] spawnerSizes = new int[5];
}
