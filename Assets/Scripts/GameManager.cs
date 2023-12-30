using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool twoPlayer = false;
    public GameObject[] craftPrefabs;   

    public Craft playerOneCraft = null;

  public BulletManager bulletManager = null;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bulletManager = GetComponent<BulletManager>();
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
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerOneCraft && playerOneCraft.craftData.shotPower < CraftConfiguration.MAX_SHOT_POWER - 1) 
                playerOneCraft.craftData.shotPower++;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (playerOneCraft)
                playerOneCraft.AddOption();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (playerOneCraft)
                playerOneCraft.IncreaseBeamStrenght();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (bulletManager) bulletManager.SpawnBullet(BulletManager.BulletType.bullet1_Size1, 0, 150
                                                        , Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
        }
    }
} 
