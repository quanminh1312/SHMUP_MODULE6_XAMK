using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : Menu
{
    public static GameOverMenu instance = null;
    public TMPro.TextMeshProUGUI scoreText = null;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one gameOver Scene");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public void OnContinueButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    public void GameOver()
    {
        TurnOn(null);
        AudioManager.instance.PlayMusic(AudioManager.Track.GameOver, true, 0.5f);
        scoreText.text = GameManager.Instance.playerDatas[0].score.ToString(); // player 2
    }
}
