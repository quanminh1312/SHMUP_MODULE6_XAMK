using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool twoPlayer = false;
    public GameObject[] craftPrefabs;   

    //public Craft playerOneCraft = null;
    public Craft[] playerCrafts = new Craft[2];
    public PlayerData[] playerDatas = new PlayerData[2];

    public BulletManager bulletManager = null;
    
    public LevelProgress progressWindow = null;

    public Session gameSession = new Session();

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
    private void Start()
    {
        Application.targetFrameRate = 60;
        playerDatas[0] = new PlayerData();
        playerDatas[1] = new PlayerData();
    }
    public void SpawnPlayer(int playerIndex, int craftType)
    {
        Debug.Assert(craftType<craftPrefabs.Length);
        Debug.Log("Spawning player " + playerIndex);
        playerCrafts[playerIndex] = Instantiate(craftPrefabs[craftType]).GetComponent<Craft>();
        playerCrafts[playerIndex].playerIndex = playerIndex;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!playerCrafts[0]) SpawnPlayer(1,0);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerCrafts[0] && playerCrafts[0].craftData.shotPower < CraftConfiguration.MAX_SHOT_POWER - 1)
                playerCrafts[0].craftData.shotPower++;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (playerCrafts[0])
                playerCrafts[0].AddOption();
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (playerCrafts[0])
                playerCrafts[0].IncreaseBeamStrenght();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnemyPattern testPattern = GameObject.FindObjectOfType<EnemyPattern>();
            testPattern.Spawn();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            if (bulletManager) bulletManager.SpawnBullet(BulletManager.BulletType.bullet1_Size1, 0, 150
                                                        , Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0,0,false);
        }
    }
    public void PickUpFallOffScreen(PickUp pickUp)
    {
        Debug.Log("PickUp fell off screen");
    }
    public void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Stage01");
    }
} 
