using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;
    internal  Menu ActiveMenu = null;
    private bool titleMenuShown = false;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than 1 menumanager");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SwitchToGameplayMenus()
    {
        SceneManager.LoadScene("ControllerMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControlSettingsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("DebugHUD", LoadSceneMode.Additive);
        SceneManager.LoadScene("GameOverMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("AudioOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("HighScoresMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("KeyPadMenu",LoadSceneMode.Additive);
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("GraphicsOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("WellDoneMenu", LoadSceneMode.Additive);
    }
    public void SwitchToMainMenuMenus()
    {
        SceneManager.LoadScene("MainMenu",LoadSceneMode.Additive);
        SceneManager.LoadScene("ControllerMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControlSettingsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("DebugHUD", LoadSceneMode.Additive);
        SceneManager.LoadScene("GameOverMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("AudioOptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("HighScoresMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("KeyPadMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("GraphicsOptionsMenu", LoadSceneMode.Additive);
        if (!titleMenuShown)
        {
            SceneManager.LoadScene("TitleScreenMenu", LoadSceneMode.Additive);
            titleMenuShown = true;
        }
        else
        {
            StartCoroutine(ShowMainMenu());
        }
    }
    IEnumerator ShowMainMenu()
    {
        while (MainMenu.instance == null)
        {
            yield return null;
        }
        MainMenu.instance.TurnOn(null);
    }
}
