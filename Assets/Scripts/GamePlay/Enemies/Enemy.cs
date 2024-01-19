using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    public EnemyData data;

    public EnemyRules[] rules;


    private EnemyPattern pattern;
    [HideInInspector]
    public EnemySection[] sections;
    public bool isBoss = false;
    private int timer;
    public int timeOut = 3600;
    private bool timedOut = false;

    private WaveTrigger owingWave = null;

    Animator animator = null;
    public string timeoutParameterName = null;
    public void SetPattern(EnemyPattern pattern)
    {
        this.pattern = pattern;
    }
    private void Start()
    {
        sections = GetComponentsInChildren<EnemySection>();
        animator = GetComponentInChildren<Animator>();
        timer = timeOut;
    }
    public void setWave(WaveTrigger wave)
    {
        owingWave = wave;
    }
    private void FixedUpdate()
    {
        //timeout
        if (isBoss)
        {
            if (timer <=0 && !timedOut)
            {
                timedOut = true;
                if (animator && timeoutParameterName != null)
                    animator.SetTrigger(timeoutParameterName);
                sections[0].EnableState("TimeOut");
            }
            else
                timer--;
        }


        data.progressTimer++;
        if (pattern)
            pattern.Calculate(transform, data.progressTimer);

        //offscreen check
        float y = transform.position.y;
        if (GameManager.Instance && GameManager.Instance.progressWindow)
            y -= GameManager.Instance.progressWindow.data.positionY;
        if (y < -200)
        {
            OutOfBound();
        }

        //update state timers
        foreach (EnemySection section in sections)
        {
            section.UpdateStateTimer();
        }
    }
    public void TimeOutDestruct()
    {
        Destroy(gameObject);
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
    public void PartDestroyed()
    {
        // go through all rules and check for parts matching rulesset
        foreach (EnemyRules rule in rules)
        {
            if (!rule.triggered)
            {
                int noOfDestroyedParts = 0;
                foreach (EnemyPart part in rule.parts)
                {
                    if (part.destroyed)
                    {
                        noOfDestroyedParts++;
                    }
                }
                if (noOfDestroyedParts >= rule.noOfPartsRequired)
                {
                    rule.triggered = true;
                    rule.ruleEvent.Invoke();
                }
            }
        }
    }
    public void Destroyed(int triggerFromRullIndex)
    {
        EnemyRules triggerRule = rules[triggerFromRullIndex];
        int playerIndex = triggerRule.parts[0].destroyByPlayer; // todo check that using the first part is ok
        if (owingWave)
        {
            owingWave.EnemyDestroyed(transform.position, playerIndex);
        }
        Destroy(gameObject);
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
