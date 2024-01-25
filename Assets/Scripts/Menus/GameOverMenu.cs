using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : Menu
{
    public static GameOverMenu instance = null;
    public TMPro.TextMeshProUGUI scoreText = null;
    public TMPro.TextMeshProUGUI highScoreText = null;
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
        if (ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[0].score, (int)GameManager.Instance.gameSession.hardness) ||
            ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[1].score, (int)GameManager.Instance.gameSession.hardness))
        {
            if (GameManager.Instance.twoPlayer)
            {
                if (ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[0].score, (int)GameManager.Instance.gameSession.hardness))
                {
                    KeyPadMenu.Instance.playerIndex = 0;
                }
                else
                {
                    KeyPadMenu.Instance.playerIndex = 1;
                }

                if (ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[0].score, (int)GameManager.Instance.gameSession.hardness) &&
                    ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[1].score, (int)GameManager.Instance.gameSession.hardness))
                {
                    KeyPadMenu.Instance.bothPlayers = true;
                }
                else
                {
                    KeyPadMenu.Instance.bothPlayers = false;
                }
            }
            else
            {
                KeyPadMenu.Instance.playerIndex = 0;
                KeyPadMenu.Instance.bothPlayers = false;
            }
            KeyPadMenu.Instance.TurnOn(null);
            TurnOff(false);
        }
        else
            SceneManager.LoadScene("MainMenuScene");
    }
    public void GameOver()
    {
        TurnOn(null);
        AudioManager.instance.PlayMusic(AudioManager.Track.GameOver, true, 0.5f);
        scoreText.text = GameManager.Instance.playerDatas[0].score.ToString(); // player 2

        if (ScoreManager.instance.IsHighScore(GameManager.Instance.playerDatas[0].score, (int)GameManager.Instance.gameSession.hardness))
        {
            highScoreText.gameObject.SetActive(true);
        }
        else
        {
            highScoreText.gameObject.SetActive(false);
        }
    }
}
