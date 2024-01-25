using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    public VideoPlayer player = null;
    void Start()
    {
        player.loopPointReached += EndReached;
    }
    void EndReached(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
