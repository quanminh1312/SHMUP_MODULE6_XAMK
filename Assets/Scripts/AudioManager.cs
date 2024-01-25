using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public AudioSource musicSource1 = null;
    public AudioSource musicSource2 = null;
    public AudioSource sfxSource = null;

    public AudioMixer mixer = null; 

    public enum Track
    {
        Level1,
        Level2,
        Boss1,
        Boss2,
        GameOver,
        Won,
        Menu,
        None
    }
    public AudioClip[] musicTracks;

    private int activeMusicSource = 0;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple AudioManager instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;

        //restore preferences
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1);
        mixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        volume = PlayerPrefs.GetFloat("EffectVolume", 1);
        mixer.SetFloat("EffectVolume", Mathf.Log10(volume) * 20);
        volume = PlayerPrefs.GetFloat("MusicVolume", 1);
        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void PlayMusic(Track track, bool fade, float fadeDuration)
    {
        if (activeMusicSource ==0 || activeMusicSource ==2)
        {
            if (!fade)
            {
                mixer.SetFloat("Music1Volume",Mathf.Log10(1)*20);
                mixer.SetFloat("Music2Volume", Mathf.Log10(0.0001f));
                musicSource2.Stop();
            }
            else
            {
                if (activeMusicSource == 0)
                {
                    mixer.SetFloat("Music1Volume", Mathf.Log10(0.0001f));
                    mixer.SetFloat("Music2Volume", Mathf.Log10(0.0001f));
                }
            }
            musicSource1.clip = musicTracks[(int)track];
            StartCoroutine(DelayPlayMusic(1));
            if (fade)
            {
                StartCoroutine(Fade(1,fadeDuration,0,1));
                if (activeMusicSource ==2)
                    StartCoroutine(Fade(2, fadeDuration, 1, 0));
            }
            activeMusicSource = 1;

        }
        else if (activeMusicSource == 1)
        {
            if (!fade)
            {
                mixer.SetFloat("Music1Volume", Mathf.Log10(0.0001f));
                mixer.SetFloat("Music2Volume", Mathf.Log10(1) * 20);
                musicSource1.Stop();
            }
            musicSource2.clip = musicTracks[(int)track];
            StartCoroutine(DelayPlayMusic(2));
            activeMusicSource = 2;
            if (fade)
            {
                StartCoroutine(Fade(2, fadeDuration, 0, 1));
                StartCoroutine(Fade(1, fadeDuration, 1, 0));
            }

        }
    }
    IEnumerator DelayPlayMusic(int sourceNo)
    {
        yield return null;
        if (sourceNo == 1)
            musicSource1.Play();
        else if (sourceNo == 2)
            musicSource2.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PauseMusic()
    {
        musicSource1.Pause();
        musicSource2.Pause();
    }
    public void ResumeMusic()
    {
        musicSource1.UnPause();
        musicSource2.UnPause();
    }
    IEnumerator Fade(int sourceIndex, float duration, float startVolume, float targetVolume)
    {
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            float newVol = Mathf.Lerp(startVolume, targetVolume, timer / duration);
            newVol = Mathf.Clamp(newVol, 0.0001f, 0.9999f);
            
            if (sourceIndex == 1)
                mixer.SetFloat("Music1Volume", Mathf.Log10(newVol) * 20);
            else if (sourceIndex == 2)
                mixer.SetFloat("Music2Volume", Mathf.Log10(newVol) * 20);

            yield return null;
        }

        if (targetVolume <=0.0001f) //stop
        {
            if (sourceIndex == 1)
                musicSource1.Stop();
            else if (sourceIndex == 2)
                musicSource2.Stop();
        }

        yield return null;
    }

}
