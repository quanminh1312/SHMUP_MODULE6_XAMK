using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerMenu : Menu
{
    public static ControllerMenu instance = null;

    public TextMeshProUGUI playerText = null;

    public int whichPlayer = 0;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one Controller Screen");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    private void Update()
    {
        if (ROOT.gameObject.activeInHierarchy)
            if (InputManager.instance.CheckForPlayerInput(whichPlayer))
            {
                TurnOff(false);
            }
    }
}
