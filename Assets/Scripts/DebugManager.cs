using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    public static DebugManager instance = null;

    private bool displaying = false;
    public GameObject Root = null;

    public Toggle invincibleToggle = null;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple DebugManager instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void ToggleHUD()
    {
        if (!displaying) //turn on
        {
            if (!Root)
            {
                Debug.LogError("No root object set for DebugManager!");
                return;
            }
            else
            {
                Root.SetActive(true);
                displaying = true;
                Time.timeScale = 0; //pause game
            }
        }
        else //turn off
        {
            if (!Root)
            {
                Debug.LogError("No root object set for DebugManager!");
                return;
            }
            else
            {
                Root.SetActive(false);
                displaying = false;
                Time.timeScale = 1; //unpause game
            }
        }
    }
    public void ToggleInvincibility()
    {
        if (invincibleToggle)
        {
            GameManager.Instance.gameSession.invincible = invincibleToggle.isOn;
        }
    }
}
