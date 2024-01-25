using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Instance = null;
    public AnimatedNumber[] playerScore = new AnimatedNumber[2];
    public AnimatedNumber topScore;
    public GameObject player2Start;
    public PlayerHUD[] playerHUDs = new PlayerHUD[2];
    private Vector2 joystickPos;
    public Image fadeScreenImage = null;

    public GameObject player2HUD;
    private void Start()
    {
        if (playerHUDs[0].joystick) joystickPos = playerHUDs[0].joystick.transform.localPosition;
        if (Instance)
        {
            Debug.LogError("more than 1 hud");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        TurnOnP2(GameManager.Instance.twoPlayer);
    }
    private void FixedUpdate()
    {
        UpdateHUD();
    }
    public void UpdateHUD()
    {
        if (!GameManager.Instance) return;

        int p1Score = GameManager.Instance.playerDatas[0].score;
        int p2Score = GameManager.Instance.playerDatas[1].score;

        //score
        if (playerScore[0])
        {
            playerScore[0].UpdateNumber(p1Score);
        }

        //top score 
        int hardness = (int)GameManager.Instance.gameSession.hardness;
        int highestScore = ScoreManager.instance.TopScore(hardness);
        if (p1Score > highestScore) topScore.UpdateNumber(p1Score);
        else if (p2Score > highestScore) topScore.UpdateNumber(p2Score);
        else topScore.UpdateNumber(highestScore);

        UpdateLives(0);
        UpdateBombs(0);
        UpdatePower(0);
        UpdateBeam(0);
        UpdateControls(0);
        UpdateStat(0);
        UpdateStageScore(0);
        UpdateProgress(0);
        UpdateChain(0);
        if (GameManager.Instance.twoPlayer)
        {
            if (player2Start) player2Start.SetActive(false);
            if (playerScore[1])
            {
                playerScore[1].UpdateNumber(p2Score);
            }
            UpdateLives(1);
            UpdateBombs(1);
            UpdatePower(1);
            UpdateBeam(1);
            UpdateControls(1);
            UpdateStat(1);
            UpdateStageScore(1);
            UpdateProgress(1);
            UpdateChain(1);
        }
        else
        {
            if (player2Start) player2Start.SetActive(true);
        }
    }
    void UpdateLives(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerData data = GameManager.Instance.playerDatas[playerIndex];
        PlayerHUD hud = playerHUDs[playerIndex];

        int lives = data.lives;
        for (int i = 0; i < hud.lives.Length; i++)
        {
            hud.lives[i].SetActive(i < lives);
        }
    }
    void UpdateBombs(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD playerHUD = playerHUDs[playerIndex];


        int t = playerHUD.bigBombs.Length;
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            for (int i = 0; i < t; i++) playerHUD.bigBombs[i].SetActive(false);
            t = playerHUD.smallBombs.Length;
            for (int i = 0; i < t; i++) playerHUD.smallBombs[i].SetActive(false);
            return;
        }
        CraftData data = GameManager.Instance.playerCrafts[playerIndex].craftData;
        int bigBombs = data.largeBombs;
        int smallBombs = data.smallBombs;
        t = playerHUD.bigBombs.Length;
        for (int i = 0; i < t; i++) playerHUD.bigBombs[i].SetActive(i < bigBombs);
        t = playerHUD.smallBombs.Length;
        for (int i = 0; i < t; i++) playerHUD.smallBombs[i].SetActive(i < smallBombs);
    }
    void UpdatePower(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];

        int t = hud.powerMark.Length;
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            for (int i = 0; i < t; i++) hud.powerMark[i].SetActive(false);
            return;
        }
        CraftData data = GameManager.Instance.playerCrafts[playerIndex].craftData;
        int power = data.shotPower;
        t = hud.powerMark.Length;
        for (int i = 0; i < t; i++) hud.powerMark[i].SetActive(i < power);
    }
    void UpdateBeam(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];

        int t = hud.beamMark.Length;
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            for (int i = 0; i < t; i++) hud.beamMark[i].SetActive(false);
            hud.beamGradient.fillAmount = 0;
            return;
        }

        CraftData data = GameManager.Instance.playerCrafts[playerIndex].craftData;
        int beam = data.beamPower;
        t = hud.beamMark.Length;
        for (int i = 0; i < t; i++) hud.beamMark[i].SetActive(i < beam);

        hud.beamGradient.fillAmount =  (float)data.beamTimer / (float)Craft.MAXIMUMBEAMCHARGE;
    }
    void UpdateControls(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];

        int t = hud.buttons.Length;
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            for (int i = 0; i < t; i++) if (hud.buttons[i]) hud.buttons[i].SetActive(false);
            if (hud.left) hud.left.SetActive(false);
            if (hud.right) hud.right.SetActive(false);
            if (hud.up) hud.up.SetActive(false);
            if (hud.down) hud.down.SetActive(false);
            if (hud.joystick) hud.joystick.SetActive(false);

            return;
        }
            
        InputState state = InputManager.instance.playerState[playerIndex];
        if (state == null) return;
        if (hud.buttons[0]) hud.buttons[0].SetActive(state.shoot);
        if (hud.buttons[1]) hud.buttons[1].SetActive(state.beam);
        if (hud.buttons[2]) hud.buttons[2].SetActive(state.bomb);
        if (hud.buttons[3]) hud.buttons[3].SetActive(state.option);

        if (hud.left) hud.left.SetActive(state.left);
        if (hud.right) hud.right.SetActive(state.right);
        if (hud.up) hud.up.SetActive(state.up);
        if (hud.down) hud.down.SetActive(state.down);

        if (hud.joystick) hud.joystick.SetActive(true);
        if (hud.joystick)hud.joystick.transform.localPosition = joystickPos + state.movement * 3;
    }
    void UpdateStat(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            hud.speedStat.fillAmount = 0;
            hud.powerStat.fillAmount = 0;
            hud.beamStat.fillAmount = 0;
            hud.optionStat.fillAmount = 0;
            hud.bombsStat.fillAmount = 0;
            return;
        }

        CraftConfiguration config = GameManager.Instance.playerCrafts[playerIndex].config;
        if (config == null) return;
        hud.speedStat.fillAmount = config.speed / CraftConfiguration.MAX_SPEED;
        hud.powerStat.fillAmount = config.bulletStrength / CraftConfiguration.MAX_SHOT_POWER;
        hud.beamStat.fillAmount = config.beamPower / CraftConfiguration.MAX_BEAM_POWER;
        hud.optionStat.fillAmount = config.optionPower / CraftConfiguration.MAX_OPTION_POWER;
        hud.bombsStat.fillAmount = config.bombPower / CraftConfiguration.MAX_BOMB_POWER;
    }
    void UpdateStageScore(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            hud.stageScore.UpdateNumber(0);
            return;
        }
        PlayerData data = GameManager.Instance.playerDatas[playerIndex];
        hud.stageScore.UpdateNumber(data.stageScore);
    }
    void UpdateProgress(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];
        if (!GameManager.Instance || !GameManager.Instance.progressWindow)
        {
            hud.progress.fillAmount = 1;
            return;
        }

        float progress = GameManager.Instance.progressWindow.data.positionY / GameManager.Instance.progressWindow.levelSize;
        hud.progress.fillAmount = 1 - progress;
    }
    void UpdateChain(int playerIndex)
    {
        Debug.Assert(playerIndex < 2);
        PlayerHUD hud = playerHUDs[playerIndex];
        if (!GameManager.Instance || !GameManager.Instance.playerCrafts[playerIndex])
        {
            hud.chainScore.UpdateNumber(0);
            hud.chainGradient.fillAmount = 0;
            return;
        }
        PlayerData data = GameManager.Instance.playerDatas[playerIndex];
        hud.chainScore.UpdateNumber(data.chain);
        hud.chainGradient.fillAmount = (float)data.chainTimer / (float)PlayerData.MAX_CHAIN;
    }
    public void TurnOnP2(bool turnOn)
    {
        if (turnOn)
        {
            player2Start.gameObject.SetActive(false);
            playerScore[1].gameObject.SetActive(true);
            player2HUD.SetActive(true);
        }
        else
        {
            player2Start.gameObject.SetActive(true);
            playerScore[1].gameObject.SetActive(false);
            player2HUD.SetActive(false);
        }
    }
    public void FadeOutScreen()
    {
        if (!fadeScreenImage) return;
        fadeScreenImage.gameObject.SetActive(true);
        fadeScreenImage.color = Color.black;
    }
    public void FadeInScreen()
    {
        if (!fadeScreenImage) return;
        fadeScreenImage.gameObject.SetActive(false);
        fadeScreenImage.color = new Color(0,0,0,0);
    }
}
[Serializable]
public class PlayerHUD
{
    public GameObject[] lives = new GameObject[5];
    public GameObject[] bigBombs = new GameObject[6];
    public GameObject[] smallBombs = new GameObject[8];
    public AnimatedNumber chainScore;
    public Image chainGradient;
    public GameObject[] powerMark = new GameObject[8];
    public GameObject[] beamMark = new GameObject[5];
    public Image beamGradient;
    public AnimatedNumber stageScore;
    public Image progress;

    public GameObject[] buttons = new GameObject[4];
    public GameObject up;
    public GameObject down;
    public GameObject left;
    public GameObject right;
    public GameObject joystick;

    public Image speedStat;
    public Image powerStat;
    public Image beamStat;
    public Image optionStat;
    public Image bombsStat;

}
