using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyState
{
    public String stateName;
    private bool active;

    [Space(10)]
    [Header("Start Events")]
    public UnityEvent eventonStart = null;

    [Space(10)]
    [Header("End Events")]
    public UnityEvent eventonEnd = null;

    [Space(10)]
    [Header("Timer Events")]
    public UnityEvent eventonTime = null;

    public bool usersTimer = false;
    public float timer = 0;
    private float currentTime = 0;

    public void Enable()
    {
        eventonStart.Invoke();
    }
    public void Disable()
    {
        eventonEnd.Invoke();
    }
}
