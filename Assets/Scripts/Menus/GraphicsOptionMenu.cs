using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsOptionMenu : Menu
{
    public static GraphicsOptionMenu Instance = null;

    public Toggle fullScreenToggle = null;
    public Button nextButton = null;
    public Button prevButton = null;
    public TMPro.TextMeshProUGUI resolutionText = null;


    bool fullScreenToApply = true;
    Resolution resolutionToApply;
    private void Start()
    {
        if (Instance)
        {
            Debug.LogError("more than 1 GraphicsOptionMenu");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (fullScreenToggle)
        {
            fullScreenToggle.isOn = ScreenManager.Instance.fullscreen;
        }
        fullScreenToApply = ScreenManager.Instance.fullscreen;
        resolutionToApply = ScreenManager.Instance.currentResolution;
        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height+" - " + resolutionToApply.refreshRateRatio;
        }

    }
    public void OnApplyButton()
    {
        ScreenManager.Instance.fullscreen = fullScreenToApply;
        Screen.fullScreen = fullScreenToApply;

        if (fullScreenToApply)
        {
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("fullscreen", 0);
        }
        PlayerPrefs.Save();
    }
    public void OnFullScreenToggle()
    {
        fullScreenToApply = !fullScreenToApply;
    }
    public void OnNextButton()
    {
        resolutionToApply = ScreenManager.Instance.FindNextResolution(resolutionToApply);
        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height + " - " + resolutionToApply.refreshRateRatio;
        }
    }
    public void OnPrevButton()
    {
        resolutionToApply = ScreenManager.Instance.FindPrevResolution(resolutionToApply);
        if (resolutionText)
        {
            resolutionText.text = resolutionToApply.width + "x" + resolutionToApply.height + " - " + resolutionToApply.refreshRateRatio;
        }
    }
    public void OnBackButton()
    {
        TurnOff(true);
    }
}
