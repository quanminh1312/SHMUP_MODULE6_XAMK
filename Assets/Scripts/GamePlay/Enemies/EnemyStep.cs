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
    public enum RotationType
    {
        INVALID,
        none,
        setangle,
        lookAhead,
        spinning,
        facePlayer,
        NOOFROTATIONTYPES
    }
    [SerializeField]
    public RotationType rotate = RotationType.lookAhead;
    [SerializeField]
    public float endAngle = 0;
    [SerializeField]
    [Range(0.01f, 4f)]
    public float angleSpeed = 1;
    [SerializeField]
    public float noOfSpins = 1;

    [SerializeField]
    public MoveMentType movement;
    [SerializeField]
    public Vector2 direction;


    [SerializeField]
    public Spline spline;


    [SerializeField]
    [Range(0.01f, 20f)]
    public float movementSpeed = 4;

    public float framesToWait = 30;
    public List<string> activateStates = new List<string>();
    public List<string> deActivateStates = new List<string>();

    public EnemyStep()
    {
        movement = MoveMentType.none;
    }
    public EnemyStep(MoveMentType movementType)
    {
        movement = movementType;
        direction = Vector2.zero;

        if (movement == MoveMentType.spline)
        {
            spline = new Spline();
        }
    }
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
        else if (movement == MoveMentType.spline)
        {
            return spline.Length() / movementSpeed;
        }
        else if (movement == MoveMentType.homing)
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
        else if (movement == MoveMentType.spline)
        {
            result += (spline.LastPoint() - spline.StartPoint());
            return result;
        }
        else if (movement == MoveMentType.homing)
        {
            if (GameManager.Instance && GameManager.Instance.playerCrafts[0])//error
                return GameManager.Instance.playerCrafts[0].transform.position;//error
            else
                return startPosition;
        }
        Debug.LogError("EndPosition unprocessed");
        return result;
    }
    public Vector2 CalculatePostion(Vector2 startPosition, float stepTime, Vector2 oldPosition, Quaternion oldAngle)
    {
        float normalisedTime = stepTime / TimeToCpmplete();
        if (normalisedTime < 0) normalisedTime = 0;
        if (movement == MoveMentType.direction)
        {
            float timeToTravel = direction.magnitude / movementSpeed;
            float ratio = 0;
            if (timeToTravel != 0)
                ratio = stepTime / timeToTravel;
            Vector2 result = startPosition + (direction * ratio);
            return result;
        }
        else if (movement == MoveMentType.none)
        {
            return startPosition;
        }
        else if (movement == MoveMentType.spline)
        {
            return spline.GetPosition(normalisedTime) + startPosition;
        }
        else if (movement == MoveMentType.homing)
        {
            Vector2 dir = (oldAngle * Vector2.down);
            Vector2 mov = dir * movementSpeed;
            Vector2 pos = oldPosition + mov;
            return pos;
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
    public float EndRotation() //todo
    {
        return endAngle;
    }
    public Quaternion CalculateRotation(float startRotation, Vector3 currentPosition, Vector3 oldPosition, float time)
    {
        float normalisedTime = time / TimeToCpmplete();
        if (normalisedTime < 0) normalisedTime = 0;
        if (rotate == RotationType.setangle)
        {
            Quaternion result = Quaternion.Euler(0, 0, endAngle);
            return result;
        }
        else if (rotate == RotationType.spinning)
        {
            float start = endAngle - (noOfSpins * 360);
            float angle = Mathf.Lerp(start, endAngle, normalisedTime);
            Quaternion result = Quaternion.Euler(0, 0, angle);
            return result;
        }
        else if (rotate == RotationType.facePlayer)
        {
            float angle = 0;
            Transform target = null;
            if (GameManager.Instance && GameManager.Instance.playerCrafts[0])//error
            {
                target = GameManager.Instance.playerCrafts[0].transform;//error
            }
            if (target)
            {
                Vector2 currentDir = (currentPosition - oldPosition).normalized;
                Vector2 targetDir = (target.transform.position - currentPosition).normalized;
                Vector2 newDir = Vector2.Lerp(currentDir, targetDir, normalisedTime);
                angle = Vector2.SignedAngle(Vector2.down, newDir);
            }

            return Quaternion.Euler(0, 0, angle);
        }
        else if (rotate == RotationType.lookAhead)
        {
            Vector2 dir = (currentPosition - oldPosition).normalized;
            float angle = Vector2.SignedAngle(Vector2.down, dir);
            return Quaternion.Euler(0, 0, angle);
        }


        return Quaternion.Euler(0, 0, 0);
    }
}
