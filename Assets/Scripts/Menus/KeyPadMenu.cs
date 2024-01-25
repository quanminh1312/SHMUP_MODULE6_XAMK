using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyPadMenu : Menu
{
    public static KeyPadMenu Instance = null;

    public TMPro.TextMeshProUGUI nameText = null;
    public TMPro.TextMeshProUGUI enterText = null;

    public int playerIndex = 0;
    public bool bothPlayers = false;

    private void Start()
    {
        if (Instance)
        {
            Debug.LogError("more than 1 keypad menu");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        enterText.text = "ennter name player " + (playerIndex + 1);
    }
    public void OnEnterButton()
    {
        ScoreManager.instance.AddScore(GameManager.Instance.playerDatas[playerIndex].score,
                                        (int)GameManager.Instance.gameSession.hardness,
                                        nameText.text);
        ScoreManager.instance.SaveScore();

        if (bothPlayers && playerIndex == 0)
        {
            playerIndex = 1;
            enterText.text = "enter name player 2";
            nameText.text = "";
        }
        else
        {
            TurnOff(false);
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    public void OnKeyPress(int key)
    {
        nameText.text += (char)key;
    }
    public void OnClearButton()
    {
        nameText.text = "";
    }
    public void OnDeleteButton()
    {
        if (nameText.text.Length > 0)
        {
            nameText.text = nameText.text.Substring(0, nameText.text.Length - 1);
        }
    }
}
