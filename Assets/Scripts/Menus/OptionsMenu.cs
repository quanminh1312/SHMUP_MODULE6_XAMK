using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionsMenu : Menu
{
    public static OptionsMenu instance = null;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("more than one Options Screen");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public void ToControlSettingsMenu()
    {
        TurnOff(false); 
        ControlSettingMenu.instance.TurnOn(this);
    }
    public void ToAudioSettingsMenu()
    {
        TurnOff(false);
        AudioOptionsMenu.instance.TurnOn(this);
    }
    public void ToPreviousMenu()
    {
        TurnOff(true);
    }
}
