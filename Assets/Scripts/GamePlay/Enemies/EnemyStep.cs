using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyStep
{
    public enum MoveMentType
    {
        INVALID,
        none,
        direction,
        spline,
        atTarget,
        homing,
        follow,
        circle,
        NOOFMOVEMENTTYPES
    }
    [SerializeField]
    public MoveMentType movement;
    [SerializeField]
    public Vector2 direction;
    [SerializeField]
    [Range(1, 20)]
    public float movementSpeed = 4;

    public float framesToWait = 30;
    public List<string> activateStates = new List<string>();
    public List<string> deActivateStates = new List<string>();
    public float TimeToCpmplete()
    {
        if (movement == MoveMentType.direction)
        {
            float timeToTravel = direction.magnitude / movementSpeed;
            return timeToTravel;
        }
        else if (movement == MoveMentType.none)
        {
            return framesToWait;
        }
        Debug.LogError("unprocessed movement type, returning 1");
        return 1;
    }
    public Vector2 EndPosition(Vector2 startPosition)
    {
        Vector2 result = startPosition;

        if (movement == MoveMentType.direction)
        {
            result += direction;
            return result;
        }
        else if (movement == MoveMentType.none)
        {
            return startPosition;
        }
        Debug.LogError("EndPosition unprocessed");
        return result;
    }
    public Vector2 CalculatePostion(Vector2 startPosition, float stepTime)
    {
        if (movement == MoveMentType.direction)
        {
            float timeToTravel = direction.magnitude / movementSpeed;
            float ratio = stepTime / timeToTravel;
            Vector2 result = startPosition + (direction * ratio);
            return result;
        }
        else if (movement == MoveMentType.none)
        {
            return startPosition;
        }
        Debug.LogError("CalculatePostion unprocessed");
        return startPosition;
    }
    public void FireActivateStates(Enemy enemy)
    {
        foreach (string state in activateStates)
        {
            enemy.EnableState(state);  
        }
    }
    public void FireDeActivateStates(Enemy enemy)
    {
        foreach (string state in deActivateStates)
        {
            enemy.DisableState(state);
        }
    }
}
