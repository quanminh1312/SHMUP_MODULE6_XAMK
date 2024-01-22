using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : Menu
{
    public static MainMenu instance = null;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one MainMenu");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public void ReturnToPreviousMenu()
    {
        TurnOff(true);
    }
    public void ToGamePlayScene()
    {
        TurnOff(false);
        GameManager.Instance.StartGame();
        //SceneManager.LoadScene("GamePlayScene");
    }
    public void ToControlSettings()
    {
        TurnOff(false);
        OptionsMenu.instance.TurnOn(this);
    }
    public void ToHighScoresMenu()
    {
        TurnOff(false);
        ScoresMenu.instance.TurnOn(this);
    }
    public void OnLoadButton()
    {
        if (SaveManager.instance.LoadExists(1))
        {
            TurnOff(false);
            SaveManager.instance.LoadGame(1);
        }
    }
    public void Quit()
    {
        #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
}
