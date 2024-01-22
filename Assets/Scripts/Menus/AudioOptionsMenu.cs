using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsMenu : Menu
{ 
    public static AudioOptionsMenu instance = null;

    public Slider masterVolSlider = null;
    public Slider musicVolSlider = null; 
    public Slider fxVolSlider = null;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple AudioOptionsMenu instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        float volume = PlayerPrefs.GetFloat("MasterVolume", 1);
        masterVolSlider.value = volume;
        volume = PlayerPrefs.GetFloat("EffectVolume", 1);
        fxVolSlider.value = volume;
        volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        musicVolSlider.value = volume;
    }
    public void OnBackButton()
    {
        TurnOff(true);
    }
    public void UpdateMasterVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }
    public void UpdateSFXVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("EffectVolume", volume);
        PlayerPrefs.Save();
    }
    public void UpdateMusicVolume(float value)
    {
        float volume = Mathf.Clamp(value, 0.0001f, 1);
        AudioManager.instance.mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }
}
