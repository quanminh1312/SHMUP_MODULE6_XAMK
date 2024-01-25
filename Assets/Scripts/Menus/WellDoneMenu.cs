using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WellDoneMenu : Menu
{
    public static WellDoneMenu instance;

    public GameObject fireWorkPreFab = null;
    float timer = 1f;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple WellDoneMenu instances!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    private void Update()
    {
        if (!fireWorkPreFab) return;
        if (ROOT!= null && ROOT.gameObject.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 1f;
                Vector3 pos = new Vector3(Random.Range(-400f, 400f), Random.Range(-500f, 500f), 0);
                Instantiate(fireWorkPreFab,pos, Quaternion.identity);
            }
        }
    }
    public void OnContinueButton()
    {
        TurnOff(false);
        SceneManager.LoadScene("MainMenuScene");
    }
}
