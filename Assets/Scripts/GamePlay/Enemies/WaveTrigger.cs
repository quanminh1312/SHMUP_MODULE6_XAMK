using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    public EnemyPattern[] patterns = null;
    public float[] delays = null;

    public int waveBonus = 0;
    public bool spawnCyclicPickUp = false;
    public PickUp[] spawnSpecificPickUp;

    public int noOfEnemies = 0;
    private void Start()
    {
        foreach (EnemyPattern pattern in patterns)
        {
            if (pattern) noOfEnemies++;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(spawnWave());
    }
    IEnumerator spawnWave()
    {
        int i =0;
        foreach (EnemyPattern pattern in patterns)
        {
            if (pattern)
            {
                pattern.owingWave = this;
                Session.Hardness hardness = GameManager.Instance.gameSession.hardness;
                if (delays != null && i < delays.Length) yield return new WaitForSeconds(delays[i]);
                if (pattern.spawnOnEasy && hardness == Session.Hardness.Easy) pattern.Spawn();
                if (pattern.spawnOnNormal && hardness == Session.Hardness.Normal) pattern.Spawn();
                if (pattern.spawnOnHard && hardness == Session.Hardness.Hard) pattern.Spawn();
                if (pattern.spawnOnInsane && hardness == Session.Hardness.Insane) pattern.Spawn();
            }
            i++;
        }
        yield return null;
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        foreach (EnemyPattern pattern in patterns)
        {
            Handles.DrawLine(transform.position, pattern.transform.position);
        }
    }
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, col.size);
        }
    }
#endif
    public void EnemyDestroyed(Vector3 pos, int playerIndex)
    {
        noOfEnemies--;
        if (noOfEnemies <=0) //none left
        {
            ScoreManager.instance.ShootableDestroyed(playerIndex, waveBonus);

            if (spawnCyclicPickUp)
            {
                PickUp spawn = GameManager.Instance.GetNextDrop();
                GameManager.Instance.SpawnPickUp(spawn, pos);
            }
            foreach (PickUp p in spawnSpecificPickUp)
            {
                GameManager.Instance.SpawnPickUp(p, pos);
            }
        }
    }
}
