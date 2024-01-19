using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySection : MonoBehaviour
{
    public List<EnemyState> states = new List<EnemyState>();
    public void EnableState(string name)
    {
        foreach (EnemyState state in states)
        {
            if (state.stateName == name)
            {
                state.Enable();
            }
        }
    }
    public void DisableState(string name)
    {
        foreach (EnemyState state in states)
        {
            if (state.stateName == name)
            {
                state.Disable();
            }
        }
    }
    public void UpdateStateTimer()
    {
        foreach (EnemyState state in states)
        {
            if (state.active)
                state.IncreaseTime();
        }
    }
    public void TimeOutMessage()
    {
        Enemy enemy = transform.parent.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.TimeOutDestruct();
        }
    }
    public void UpdateStateTimers()
    {
        foreach (EnemyState state in states)
        {
            if (state.active)
                state.IncreaseTime();
        }
    }
}
