using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlSettingMenu : Menu
{
    public static ControlSettingMenu instance = null;
    public Button[] p1_buttons = new Button[8];
    public Button[] p2_buttons = new Button[8];
    public Button[] p1_keys    = new Button[12];
    public Button[] p2_keys    = new Button[12];

    public GameObject bindingPanel = null;  
    public TextMeshProUGUI bindText = null;
    public EventSystem eventSystem = null;

    private bool bindingKey = false;
    private bool bindingAxis = false;
    private bool bindingButton = false;

    private int actionBinding = -1;
    private int playerBinding = -1;

    private bool waiting = false;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one Control Settings Screen");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    private void OnEnable()
    {
        UpdateButtons();
    }
    public void ReturnToPreviousMenu()
    {
        TurnOff(true);
    }
    void UpdateButtons()
    {
        // joystick button
        for (int b=0; b<8;b++)
        {
            p1_buttons[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetButtonName(0, b);
            p2_buttons[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetButtonName(1, b);
        }

        // key buttons
        for (int b = 0; b < 8; b++)
        {
            p1_keys[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyName(0, b);
            p2_keys[b].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyName(1, b);
        }

        // key axes
        for (int b = 0; b < 4; b++)
        {
            p1_keys[b + 8].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyAxisName(0, b);
            p2_keys[b + 8].GetComponentInChildren<TextMeshProUGUI>().text = InputManager.instance.GetKeyAxisName(1, b);
        }
    }
    public void BindP1Key(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 1 "+ InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = false;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;
        waiting = true;
    }
    public void BindP1AxisKey(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 1 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = true;
        bindingButton = false;
        playerBinding = 0;
        actionBinding = actionID;
        waiting = true;
    }
    public void BindP1Button(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player 1 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = false;
        bindingButton = true;
        playerBinding = 0;
        actionBinding = actionID;
        waiting = true;
    }
    public void BindP2Key(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = true;
        bindingAxis = false;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;
        waiting = true;
    }
    public void BindP2AxisKey(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a key for Player 2 " + InputManager.axisNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = true;
        bindingButton = false;
        playerBinding = 1;
        actionBinding = actionID;
        waiting = true;
    }
    public void BindP2Button(int actionID)
    {
        eventSystem.gameObject.SetActive(false);
        bindText.text = "Press a button for Player 2 " + InputManager.actionNames[actionID];
        bindingPanel.SetActive(true);

        bindingKey = false;
        bindingAxis = false;
        bindingButton = true;
        playerBinding = 1;
        actionBinding = actionID;
        waiting = true;
    }
    private void Update()
    {
        if (bindingKey ||bindingButton || bindingAxis)
        {
            if (waiting)
            {
                if (Input.anyKey) return;
                if (InputManager.instance.DetectControllerButton(false)> -1) return;

                waiting = false;
            } 
            else {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    bindingPanel.SetActive(false);
                    bindingKey = false;
                    bindingAxis = false;
                    bindingButton=false;
                    eventSystem.gameObject.SetActive(true);
                }
                if (bindingKey || bindingAxis)
                {
                    foreach (KeyCode key in KeyCode.GetValues(typeof(KeyCode)))
                    {
                        if (!key.ToString().Contains("Joystick"))
                        {
                            if (Input.GetKeyDown(key))//key pressed
                            {
                                if (bindingAxis)
                                    InputManager.instance.BindPlayerAxisKey(playerBinding, actionBinding, key);
                                else
                                    InputManager.instance.BindPlayerKey(playerBinding, actionBinding, key);
                                bindingPanel.SetActive(false);
                                bindingKey = false;
                                bindingAxis = false;
                                eventSystem.gameObject.SetActive(true);
                                UpdateButtons();
                            }
                        }
                    }
                }
                else if (bindingButton)
                {
                    int button = InputManager.instance.DetectControllerButton(false);
                    if (button > -1)//button pressed
                    {
                        InputManager.instance.BindPlayerButton(playerBinding, actionBinding, (byte)button);
                        bindingPanel.SetActive(false);
                        bindingKey = false;
                        bindingButton = false;
                        eventSystem.gameObject.SetActive(true);
                        UpdateButtons();
                    }
                }
            }
        }
    }
}
