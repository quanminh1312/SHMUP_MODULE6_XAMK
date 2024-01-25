using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        INVALID,
        InMenus,
        Playing,
        Paused,
    }
    public GameState gameState = GameState.INVALID;
    public static GameManager Instance = null;
    public bool twoPlayer = false;
    public GameObject[] craftPrefabs;

    //public Craft playerOneCraft = null;
    public Craft[] playerCrafts = new Craft[2];
    public PlayerData[] playerDatas = new PlayerData[2];

    public BulletManager bulletManager = null;

    public LevelProgress progressWindow = null;

    public Session gameSession = new Session();

    public PickUp[] cyclicDrops = new PickUp[15];
    public PickUp[] Medals = new PickUp[10];
    private int currentDropIndex = 0;
    private int currentMedalIndex = 0;

    public PickUp option = null;
    public PickUp powerUp = null;
    public PickUp beamUp = null;
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
        Debug.Assert(craftType < craftPrefabs.Length);
        Debug.Log("Spawning player " + playerIndex);
        int whichPrefab = playerIndex;//todo on the craft select menu, this is the selected ship
        playerCrafts[playerIndex] = Instantiate(craftPrefabs[whichPrefab]).GetComponent<Craft>();
        playerCrafts[playerIndex].playerIndex = playerIndex;
        if (Instance.twoPlayer)
            if (playerIndex == 0)
                gameSession.craftDatas[playerIndex].positionX = -50;
            else
                gameSession.craftDatas[playerIndex].positionX = 50;
    }
    public void SpawnPlayers()
    {
        SpawnPlayer(0, 0); //todo craft type
        if (twoPlayer) SpawnPlayer(1, 1);
    }

    public void DelayedRespawn(int playerIndex)
    {
        StartCoroutine(RespawnCoroutine(playerIndex));
    }
    IEnumerator RespawnCoroutine(int playerIndex)
    {
        yield return new WaitForSeconds(1.5f);
        SpawnPlayer(playerIndex, 0); //todo craft type

        yield return null;
    }
    public void ResetState(int playerIndex)
    {
        CraftData craftData = gameSession.craftDatas[playerIndex];
        craftData.positionX = 0;
        craftData.positionY = 0;
        craftData.shotPower = 0;
        craftData.noOfEnableOptions = 0;
        craftData.optionsLayout = 0;
        craftData.beamFiring = false;
        craftData.beamCharge = 0;
        craftData.beamTimer = 0;
        craftData.beamPower = 0;
        craftData.smallBombs = 3;
        craftData.largeBombs = 0;
    }
    public void RestoreState(int playerIndex)
    {
        int number = gameSession.craftDatas[playerIndex].noOfEnableOptions;
        gameSession.craftDatas[playerIndex].noOfEnableOptions = 0;
        gameSession.craftDatas[playerIndex].positionX = 0;
        gameSession.craftDatas[playerIndex].positionY = 0;
        for (int i = 0; i < number; i++)
        {
            playerCrafts[playerIndex].AddOption(0);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (playerCrafts[0] && gameSession.craftDatas[0].shotPower < CraftConfiguration.MAX_SHOT_POWER - 1)
                gameSession.craftDatas[0].shotPower++;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (playerCrafts[0])
                playerCrafts[0].AddOption(0);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (playerCrafts[0])
                playerCrafts[0].IncreaseBeamStrenght(0);
        }
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugManager.instance.ToggleHUD();
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AudioManager.instance.PlayMusic(AudioManager.Track.Level1, true, 2f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AudioManager.instance.PlayMusic(AudioManager.Track.Boss1, true, 2f);
        }
    }
    public void TogglePause()
    {
        if (gameState == GameState.Playing) // pause the game
        {
            gameState = GameState.Paused;
            AudioManager.instance.PauseMusic();
            PauseMenu.instance.TurnOn(null);
            if (DebugManager.instance.displaying)
            {
                DebugManager.instance.ToggleHUD();
            }
            Time.timeScale = 0;
        }
        else // unpaused
        {
            gameState = GameState.Playing;
            AudioManager.instance.ResumeMusic();
            PauseMenu.instance.TurnOff(false);
            Time.timeScale = 1;
        }
    }
    public void PickUpFallOffScreen(PickUp pickUp)
    {
        if (pickUp.config.type == PickUp.PickUpType.Medal)
        {
            currentMedalIndex = 0;
        }
    }
    public PickUp GetNextDrop()
    {
        PickUp result = cyclicDrops[currentDropIndex];

        if (result.config.type == PickUp.PickUpType.Medal)
        {
            result = Medals[currentMedalIndex];
            currentMedalIndex++;
            if (currentMedalIndex > 9) currentMedalIndex = 0;
        }


        currentDropIndex++;
        if (currentDropIndex > 14) currentDropIndex = 0;
        return result;
    }
    public PickUp SpawnPickUp(PickUp pickUpPrefab, Vector2 pos)
    {
        PickUp p = Instantiate(pickUpPrefab, pos, Quaternion.identity);
        if (p)
        {
            p.transform.SetParent(GameManager.Instance.transform);
        }
        return p;
    }
    public void StartGame()
    {
        gameState = GameState.Playing;
        ResetState(0);
        if (twoPlayer) ResetState(1);
        playerDatas[0].ResetData();
        playerDatas[1].ResetData();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Stage01");
    }
    public void resumeGameFromLoad()
    {
        gameState = GameState.Playing;
        switch(gameSession.stage)
        {
            case 1:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage01");
                break;
            case 2:
                UnityEngine.SceneManagement.SceneManager.LoadScene("Stage02");
                break;
        }
    }
    public void NextStage()
    {
        HUD.Instance.FadeOutScreen();
        if (gameSession.stage == 1)
        {
            gameSession.stage = 2;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Stage02");
        }
        else if (gameSession.stage == 2)
        {
            WellDoneMenu.instance.TurnOn(null);
        }
        HUD.Instance.FadeInScreen();
    }
} 
