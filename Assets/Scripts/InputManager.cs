using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance = null;

    public InputState[] playerState = new InputState[2];
    public InputState[] prePlayerState = new InputState[2];

    public ButtonMapping[] playerButtons = new ButtonMapping[2];
    public AxisMapping[] playerAxis = new AxisMapping[2];

    public KeyButtonMapping[] playerKeyButtons = new KeyButtonMapping[2];
    public KeyAxisMapping[] playerKeyAxis = new KeyAxisMapping[2];

    public int[] playerController = new int[2];
    public bool[] playerUsingKeys = new bool[2];

    public const float deadZone = 0.01f;

    private System.Array allKeyCodes = System.Enum.GetValues(typeof(KeyCode));


    private string[,] playerButtonNames = { {"J1_B1", "J1_B2", "J1_B3", "J1_B4", "J1_B5", "J1_B6", "J1_B7", "J1_B8" },
                                            {"J2_B1", "J2_B2", "J2_B3", "J2_B4", "J2_B5", "J2_B6", "J2_B7", "J2_B8" },
                                            {"J3_B1", "J3_B2", "J3_B3", "J3_B4", "J3_B5", "J3_B6", "J3_B7", "J3_B8" },
                                            {"J4_B1", "J4_B2", "J4_B3", "J4_B4", "J4_B5", "J4_B6", "J4_B7", "J4_B8" },
                                            {"J5_B1", "J5_B2", "J5_B3", "J5_B4", "J5_B5", "J5_B6", "J5_B7", "J5_B8" },
                                            {"J6_B1", "J6_B2", "J6_B3", "J6_B4", "J6_B5", "J6_B6", "J6_B7", "J6_B8" }};

    private string[,] playerAxisNames = {   {"J1_Horizontal","J1_Vertical" },
                                            {"J2_Horizontal","J2_Vertical" },
                                            {"J3_Horizontal","J3_Vertical" },
                                            {"J4_Horizontal","J4_Vertical" },
                                            {"J5_Horizontal","J5_Vertical" },
                                            {"J6_Horizontal","J6_Vertical" }};

    public string[] oldJoyStick = null;

    public static string[] actionNames = { "Shoot", "Bomb", "Option", "Auto", "Beam", "Menu", "Extra2", "Extra3" };
    public static string[] axisNames = { "left", "right", "up", "down" };

    private void Awake()
    {
        if (instance)
        {
            Debug.LogError("more than 1 Input manager");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //initialization
        playerController[0] = -1;
        playerController[1] = -1;

        playerUsingKeys[0] = false;
        playerUsingKeys[1] = false;

        playerAxis[0] = new AxisMapping();
        playerAxis[1] = new AxisMapping();

        playerButtons[0] = new ButtonMapping();
        playerButtons[1] = new ButtonMapping();

        playerKeyButtons[0] = new KeyButtonMapping();
        playerKeyButtons[1] = new KeyButtonMapping();

        playerKeyAxis[0] = new KeyAxisMapping();
        playerKeyAxis[1] = new KeyAxisMapping();

        playerState[0] = new InputState();
        playerState[1] = new InputState();

        prePlayerState[0] = new InputState();
        prePlayerState[1] = new InputState();

        oldJoyStick = Input.GetJoystickNames();

        StartCoroutine(CheckController());
    }
    IEnumerator CheckController()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            string[] currentJoySticks = Input.GetJoystickNames();

            for (int i = 0; i < currentJoySticks.Length; i++)
            {
                if (i < oldJoyStick.Length)
                {
                    if (currentJoySticks[i] != oldJoyStick[i])
                    {
                        if (string.IsNullOrEmpty(currentJoySticks[i])) // disconnect
                        {
                            Debug.Log("Controller " + i + " is has been disconnected");
                            if (playerIsUsingController(i))
                            {
                                ControllerMenu.instance.whichPlayer = i;
                                ControllerMenu.instance.playerText.text = "Player " + (i + 1) + " Controller is disconnected";
                                ControllerMenu.instance.TurnOn(null);
                                //gamemanager pause game play
                            }
                        }
                        else //new connect
                        {
                            Debug.Log("Controller " + i + " is connected using: " + currentJoySticks[i]);
                        }
                    }
                }
                else
                {
                    Debug.Log("new controller connected");
                }
            }
        }
    }
    private bool playerIsUsingController(int i)
    {
        if (playerController[0] == i)
            return true;
        if (GameManager.Instance.twoPlayer && playerController[1] == i)
            return true;
        return false;
    }
    void updatePlayerState(int playerIndex)
    {
        prePlayerState[playerIndex].option = playerState[playerIndex].option;
        prePlayerState[playerIndex].beam = playerState[playerIndex].beam;
        prePlayerState[playerIndex].auto = playerState[playerIndex].auto;
        prePlayerState[playerIndex].bomb = playerState[playerIndex].bomb;
        prePlayerState[playerIndex].shoot = playerState[playerIndex].shoot;

        playerState[playerIndex].left = false;
        playerState[playerIndex].right = false;
        playerState[playerIndex].down = false;
        playerState[playerIndex].up = false;
        playerState[playerIndex].shoot = false;
        playerState[playerIndex].bomb = false;
        playerState[playerIndex].option = false;
        playerState[playerIndex].beam = false;
        playerState[playerIndex].auto = false;
        playerState[playerIndex].menu = false;
        playerState[playerIndex].extra2 = false;
        playerState[playerIndex].extra3 = false;


        if (Input.GetKey(playerKeyAxis[playerIndex].left)) playerState[playerIndex].left = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].right)) playerState[playerIndex].right = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].up)) playerState[playerIndex].up = true;
        if (Input.GetKey(playerKeyAxis[playerIndex].down)) playerState[playerIndex].down = true;

        if (Input.GetKey(playerKeyButtons[playerIndex].shoot)) playerState[playerIndex].shoot = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].bomb)) playerState[playerIndex].bomb = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].option)) playerState[playerIndex].option = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].beam)) playerState[playerIndex].beam = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].auto)) playerState[playerIndex].auto = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].menu)) playerState[playerIndex].menu = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].extra2)) playerState[playerIndex].extra2 = true;
        if (Input.GetKey(playerKeyButtons[playerIndex].extra3)) playerState[playerIndex].extra3 = true;


        if (playerController[playerIndex] < 0)
        {
            UpdateMovement(playerIndex);
            return;
        }

        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].horizontal]) < deadZone)
            playerState[playerIndex].left = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].horizontal]) > -deadZone)
            playerState[playerIndex].right = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].vertical]) < deadZone)
            playerState[playerIndex].down = true;
        if (Input.GetAxisRaw(playerAxisNames[playerController[playerIndex], playerAxis[playerIndex].vertical]) > -deadZone)
            playerState[playerIndex].up = true;


        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].shoot]))
            playerState[playerIndex].shoot = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].bomb]))
            playerState[playerIndex].bomb = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].option]))
            playerState[playerIndex].option = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].beam]))
            playerState[playerIndex].beam = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].auto]))
            playerState[playerIndex].auto = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].menu]))
            playerState[playerIndex].menu = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].extra2]))
            playerState[playerIndex].extra2 = true;
        if (Input.GetButton(playerButtonNames[playerController[playerIndex], playerButtons[playerIndex].extra3]))
            playerState[playerIndex].extra3 = true;

        UpdateMovement(playerIndex);
    }
    private void FixedUpdate()
    {
        updatePlayerState(0);
        if (GameManager.Instance && GameManager.Instance.twoPlayer)
            updatePlayerState(1);

    }
    public int DetectControllerButton(bool IsController) //return controller index or button index
    {
        for (int j = 0; j < 6; j++)
        {
            for (int b = 0; b < 8; b++)
            {
                if (Input.GetButton(playerButtonNames[j, b]))
                { 
                    if (IsController) return j;
                    return b;
                }
            }
        }
        return -1;
    }
    public int DetectKeyPress()
    {
        foreach (KeyCode key in allKeyCodes)
        {
            if (Input.GetKey(key)) return (int)key;
        }
        return -1;
    }
    public bool CheckForPlayerInput(int playerIndex)
    {
        int controller = DetectControllerButton(true);
        if (controller > -1)
        {
            playerController[playerIndex] = controller;
            playerUsingKeys[playerIndex] = false;
            Debug.Log("player " + playerIndex + " is set to controller " + controller);
            return true;
        }
        if (DetectKeyPress() > -1)
        {
            playerController[playerIndex] = -1;
            playerUsingKeys[playerIndex] = true;
            Debug.Log("player " + playerIndex + " is set to keyboard");
            return true;
        }
        return false;
    }
    public string GetButtonName(int playerindex, int actionId)
    {
        string buttonName = "";
        switch (actionId)
        {
            case 0:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].shoot];
                break;
            case 1:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].bomb];
                break;
            case 2:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].option];
                break;
            case 3:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].auto];
                break;
            case 4:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].beam];
                break;
            case 5:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].menu];
                break;
            case 6:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].extra2];
                break;
            case 7:
                buttonName = playerButtonNames[playerindex, playerButtons[playerindex].extra3];
                break;
        }
        char b = buttonName[4];
        return "Button " + b.ToString();
    }
    public string GetKeyName(int playerindex, int actionId)
    {
        KeyCode keyCode = KeyCode.None;
        switch (actionId)
        {
            case 0:
                keyCode = playerKeyButtons[playerindex].shoot;
                break;
            case 1:
                keyCode = playerKeyButtons[playerindex].bomb;
                break;
            case 2:
                keyCode = playerKeyButtons[playerindex].option;
                break;
            case 3:
                keyCode = playerKeyButtons[playerindex].auto;
                break;
            case 4:
                keyCode = playerKeyButtons[playerindex].beam;
                break;
            case 5:
                keyCode = playerKeyButtons[playerindex].menu;
                break;
            case 6:
                keyCode = playerKeyButtons[playerindex].extra2;
                break;
            case 7:
                keyCode = playerKeyButtons[playerindex].extra3;
                break;
        }
        return keyCode.ToString();
    }
    public string GetKeyAxisName(int playerindex, int actionId)
    {
        KeyCode keyCode = KeyCode.None;
        switch (actionId)
        {
            case 0:
                keyCode = playerKeyAxis[playerindex].left;
                break;
            case 1:
                keyCode = playerKeyAxis[playerindex].right;
                break;
            case 2:
                keyCode = playerKeyAxis[playerindex].up;
                break;
            case 3:
                keyCode = playerKeyAxis[playerindex].down;
                break;
        }
        return keyCode.ToString();
    }
    public void BindPlayerKey(int playerIndex, int actionID, KeyCode key)
    {
        switch(actionID)
        {
            case 0:
                playerKeyButtons[playerIndex].shoot = key;
                break;
            case 1:
                playerKeyButtons[playerIndex].bomb = key;
                break;
            case 2:
                playerKeyButtons[playerIndex].option = key;
                break;
            case 3:
                playerKeyButtons[playerIndex].auto = key;
                break;
            case 4:
                playerKeyButtons[playerIndex].beam = key;
                break;
            case 5:
                playerKeyButtons[playerIndex].menu = key;
                break;
            case 6:
                playerKeyButtons[playerIndex].extra2 = key;
                break;
            case 7:
                playerKeyButtons[playerIndex].extra3 = key;
                break;
        }
    }
    public void BindPlayerButton(int playerIndex, int actionID, byte button)
    {
        switch (actionID)
        {
            case 0:
                playerButtons[playerIndex].shoot = button;
                break;
            case 1:
                playerButtons[playerIndex].bomb = button;
                break;
            case 2:
                playerButtons[playerIndex].option = button;
                break;
            case 3:
                playerButtons[playerIndex].auto = button;
                break;
            case 4:
                playerButtons[playerIndex].beam = button;
                break;
            case 5:
                playerButtons[playerIndex].menu = button;
                break;
            case 6:
                playerButtons[playerIndex].extra2 = button;
                break;
            case 7:
                playerButtons[playerIndex].extra3 = button;
                break;
        }
    }
    public void BindPlayerAxisKey(int playerIndex, int actionID, KeyCode key)
    {
        switch (actionID)
        {
            case 0:
                playerKeyAxis[playerIndex].left = key;
                break;
            case 1:
                playerKeyAxis[playerIndex].right = key;
                break;
            case 2:
                playerKeyAxis[playerIndex].up = key;
                break;
            case 3:
                playerKeyAxis[playerIndex].down= key;
                break;
        }
    }
    public void UpdateMovement(int playerIndex)
    {
        playerState[playerIndex].movement.x = 0;
        playerState[playerIndex].movement.y = 0;

        if (playerState[playerIndex].right)
        {
            playerState[playerIndex].movement.x += 1;
        }
        if (playerState[playerIndex].left)
        {
            playerState[playerIndex].movement.x += -1;
        }


        if (playerState[playerIndex].up)
        {
            playerState[playerIndex].movement.y += 1;
        }
        if (playerState[playerIndex].down)
        {
            playerState[playerIndex].movement.y += -1;
        }

        playerState[playerIndex].movement.Normalize();
    }
}
public class InputState
{
    public Vector2 movement;
    public bool left,right,up,down;  
    public bool shoot,bomb,option,auto,beam,menu,extra2,extra3;
}
public class ButtonMapping
{
    public byte shoot  = 0;
    public byte bomb   = 1;
    public byte option = 2;
    public byte auto   = 3; 
    public byte beam   = 4;
    public byte menu   = 5;
    public byte extra2 = 6;  
    public byte extra3 = 7;

}
public class AxisMapping
{
    public byte horizontal  = 0;
    public byte vertical    = 1;
}
public class KeyButtonMapping
{
    public KeyCode shoot = KeyCode.B;
    public KeyCode bomb = KeyCode.N;
    public KeyCode option = KeyCode.M;
    public KeyCode auto = KeyCode.Comma;
    public KeyCode beam = KeyCode.Period;
    public KeyCode menu = KeyCode.J;
    public KeyCode extra2 = KeyCode.K;
    public KeyCode extra3 = KeyCode.L;

}
public class KeyAxisMapping
{
    public KeyCode left  = KeyCode.LeftArrow;
    public KeyCode right = KeyCode.RightArrow;
    public KeyCode up    = KeyCode.UpArrow;
    public KeyCode down  = KeyCode.DownArrow;
}