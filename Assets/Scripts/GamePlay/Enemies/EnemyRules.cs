using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyRules
{
    [HideInInspector]
    public bool triggered;
    public int noOfPartsRequired;   
    public List<EnemyPart> parts = new List<EnemyPart>();

    [Space(10)]
    [Header("On rule Triggered")]
    [Space(10)] 
    public UnityEvent ruleEvent = null;

    public List<int> eventDelays = new List<int>();

}
