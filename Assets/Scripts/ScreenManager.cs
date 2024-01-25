using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance = null;
    public bool fullscreen = true;
    public Resolution currentResolution;
    Resolution[] allResolutions;
    void Start()
    {
        if (Instance)
        {
            Debug.LogError("more than 1 ScreenManager");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        currentResolution = Screen.currentResolution;
        allResolutions = Screen.resolutions;
    }
    public void SetResolution(Resolution res)
    {
        if (fullscreen)
            Screen.SetResolution(res.width, res.height,FullScreenMode.ExclusiveFullScreen, res.refreshRateRatio);
        else
            Screen.SetResolution(res.width, res.height, FullScreenMode.Windowed, res.refreshRateRatio);

        PlayerPrefs.SetInt("resolutionWidth", res.width);
        PlayerPrefs.SetInt("resolutionHeight", res.height);
        PlayerPrefs.SetInt("refreshRate", res.refreshRate);
        PlayerPrefs.Save();
    }
     
    void RestoreSettings()
    {
        //restore resolution
        int width = PlayerPrefs.GetInt("resolutionWidth",1920);
        int height = PlayerPrefs.GetInt("resolutionHeight",1080);
        int refreshRate = PlayerPrefs.GetInt("refreshRate",60);
        Resolution res = FindResolution(width,height,refreshRate);
        SetResolution(res);

        //restore fullscreen
        if (PlayerPrefs.HasKey("fullscreen"))
        {
            int fullScreenInt = PlayerPrefs.GetInt("fullscreen");
            if (fullScreenInt == 0)
            {
                fullscreen = false;
            }
            else if (fullScreenInt == 1)
            {
                fullscreen = true;
            }
        }
        Screen.fullScreen = fullscreen;
    }
    Resolution FindResolution(int width, int height, int refreshRate)
    {
        foreach (Resolution res in allResolutions)
        {
            if (res.width == width && res.height == height && res.refreshRate == refreshRate)
            {
                return res;
            }
        }
        return currentResolution;
    }

    public Resolution FindNextResolution(Resolution resolutionToApply)
    {
        int currentIndex = FindResolutionIndex(currentResolution);
        currentIndex++;

        if (currentIndex >= allResolutions.Length)
        {
            currentIndex = 0;
        }
        return allResolutions[currentIndex];
    }

    public Resolution FindPrevResolution(Resolution resolutionToApply)
    {
        int currentIndex = FindResolutionIndex(currentResolution);
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = allResolutions.Length - 1;
        }
        return allResolutions[currentIndex];
    }
    int FindResolutionIndex(Resolution resolution)
    {
        for (int i = 0; i < allResolutions.Length; i++)
        {
            if (allResolutions[i].width == resolution.width && 
                allResolutions[i].height == resolution.height && 
                allResolutions[i].refreshRate == resolution.refreshRate)
            {
                return i;
            }
        }
        return -1;
    }
}
