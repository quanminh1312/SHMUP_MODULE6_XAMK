using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Menu
{
    public static PauseMenu instance = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple PauseMenu instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void OnResumeButton()
    {
        GameManager.Instance.TogglePause();
    }
    public void OnSaveButton()
    {
        SaveManager.instance.SaveGame(1);
    }
    public void OnLoadButton()
    {
        Time.timeScale = 1;
        if (SaveManager.instance.LoadExists(1))
        {
            SaveManager.instance.LoadGame(1);
        }
    }
    public void OnOptionsButton()
    {
        TurnOff(false);
        OptionsMenu.instance.TurnOn(this);
    }
    public void OnMainMenuButton()
    {
        Time.timeScale = 1;
        TurnOff(false);
        SceneManager.LoadScene("MainMenuScene");
    }
}
