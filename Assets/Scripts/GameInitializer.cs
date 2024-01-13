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
                    break;
                case GameMode.GamePlay:
                    MenuManager.instance.SwitchToGameplayMenus();
                    break;
            }
        }
        menuLoaded = true;
    }
}
