using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public EnemyData data;
    private EnemyPattern pattern;
    public EnemySection[] sections;
    public void SetPattern(EnemyPattern pattern)
    {
        this.pattern = pattern;
    }
    private void Start()
    {
        sections = GetComponentsInChildren<EnemySection>();
    }
    private void FixedUpdate()
    {
        data.progressTimer++;
        pattern.Calculate(transform, data.progressTimer);

        //offscreen check
        float y = transform.position.y;
        if (GameManager.Instance && GameManager.Instance.progressWindow)
            y -= GameManager.Instance.progressWindow.data.positionY;
        if (y < -200)
        {
            OutOfBound();
        }
    }
    public void OutOfBound()
    {
        Destroy(gameObject);
    }
    public void EnableState(String name)
    {
        foreach (EnemySection section in sections)
        {
            section.EnableState(name);
        }
    }
    public void DisableState(String name)
    {
        foreach (EnemySection section in sections)
        {
            section.DisableState(name);
        }
    }
}

[Serializable]  
public struct EnemyData
{
    public float progressTimer;

    public float positionX;
    public float positionY;

    public int patternUID;
}
