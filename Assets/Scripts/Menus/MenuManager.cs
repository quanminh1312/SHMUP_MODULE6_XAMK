using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance = null;
    internal  Menu ActiveMenu = null;
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
    }
    public void SwitchToMainMenuMenus()
    {
        SceneManager.LoadScene("MainMenu",LoadSceneMode.Additive);
        SceneManager.LoadScene("TitleScreenMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControllerMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("OptionsMenu", LoadSceneMode.Additive);
        SceneManager.LoadScene("ControlSettingsMenu", LoadSceneMode.Additive);
    }
}
