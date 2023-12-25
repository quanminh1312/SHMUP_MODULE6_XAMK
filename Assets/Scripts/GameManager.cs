using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool twoPlayer = false;
    public GameObject[] craftPrefabs;   

    public Craft playerOneCraft = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SpawnPlayer(int playerIndex, int craftType)
    {
        Debug.Assert(craftType<craftPrefabs.Length);
        Debug.Log("Spawning player " + playerIndex);
        playerOneCraft = Instantiate(craftPrefabs[craftType]).GetComponent<Craft>();
        playerOneCraft.playerIndex = playerIndex;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!playerOneCraft) SpawnPlayer(1,0);
        }

        if (Input .GetKeyDown(KeyCode.B))
        {
            if (playerOneCraft) playerOneCraft.Explode();
        }
    }
} 
