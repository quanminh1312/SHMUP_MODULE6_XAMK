using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Awake()
    {
        if (GameManager.Instance == null) 
        {
            if (gameManagerPrefab)
            {
                Instantiate(gameManagerPrefab);
            }
            else
                Debug.Log("no gamemanagerprefab");
        }
    }
    private void Update()
    {
        if (!menuLoaded) 
        {
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
