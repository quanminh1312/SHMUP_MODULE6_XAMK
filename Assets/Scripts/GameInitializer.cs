using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    public enum GameMode
    {
        INVALID,
        Menus,
        GamePlay
    }
    public GameMode gameMode;
    public GameObject gameManagerPrefab = null;
    private bool menuLoaded = false;
    private Scene displayScrene;

    public int stageNumber = 0;
    public AudioManager.Track playMusicTrack = AudioManager.Track.None;
    private void Awake()
    {
        if (GameManager.Instance == null) 
        {
            if (gameManagerPrefab)
            {
                Instantiate(gameManagerPrefab);
                displayScrene = SceneManager.GetSceneByName("DisplayScene");
            }
            else
                Debug.Log("no gamemanagerprefab");
        }
    }
    private void Update()
    {
        if (!menuLoaded)
        {
            if (!displayScrene.isLoaded)
            {
                SceneManager.LoadScene("DisplayScene", LoadSceneMode.Additive);
            }
            switch (gameMode)
            {
                case GameMode.Menus:
                    MenuManager.instance.SwitchToMainMenuMenus();
                    GameManager.Instance.gameState = GameManager.GameState.InMenus;
                    break;
                case GameMode.GamePlay:
                    MenuManager.instance.SwitchToGameplayMenus();
                    GameManager.Instance.gameState = GameManager.GameState.Playing;
                    GameManager.Instance.gameSession.stage = stageNumber;
                    break;
            }

            if (playMusicTrack != AudioManager.Track.None)
            {
                AudioManager.instance.PlayMusic(playMusicTrack, true, 1);
            }

            if (gameMode == GameMode.GamePlay)
            {
                SaveManager.instance.SaveGame(0); // 0 = autosave at beginning of state
                GameManager.Instance.SpawnPlayers();
            }

            menuLoaded = true;
        }
    }
}
