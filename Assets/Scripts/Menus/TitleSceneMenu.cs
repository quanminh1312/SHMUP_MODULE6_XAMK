using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : Menu
{
    public static TitleScene instance = null;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one TitleScreen");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    private void Update()
    {
        if (ROOT.gameObject.activeInHierarchy)
            if (InputManager.instance.CheckForPlayerInput(0))
            {
                TurnOff(false);
                MainMenu.instance.TurnOn(this);
            }
    }
}
