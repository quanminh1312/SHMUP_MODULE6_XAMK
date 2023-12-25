using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CraftConfig",menuName ="SHMUP/CraftConfiguration")]
public class CraftConfiguration : ScriptableObject
{
    public float speed;
    public float bulletStrength;
    public float beamStrength;
    public Sprite craftSprite;
}
