using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoresMenu : Menu
{
    enum Hardness { Easy, Medium, Hard, Expert };
    public static ScoresMenu instance;
    public TMPro.TextMeshProUGUI[] scores = null;
    public TMPro.TextMeshProUGUI[] names = null;
    public TMPro.TextMeshProUGUI hardnessText = null;


    int hardness = 0;

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple ScoresMenu instances");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void OnNextButton()
    {
        hardness++;
        if (hardness > 3) hardness = 3;
        hardnessText.text = ((Hardness)hardness).ToString();
        UpdateScores();
    }
    public void OnPrevButton()
    { 
        hardness--;
        if (hardness < 0) hardness = 0;
        hardnessText.text = ((Hardness)hardness).ToString();
        UpdateScores();
    }
    public void OnFriendsButton()
    {

    }
    public void OnReturnButton()
    {
        TurnOff(true);
    }
    public override void TurnOn(Menu previous)
    {
        base.TurnOn(previous);
        UpdateScores();
    }
    public void UpdateScores()
    {
        for (int s = 0; s < 8; s++)
        {
            scores[s].text = ScoreManager.instance.scores[s, hardness].ToString();
            names[s].text = ScoreManager.instance.names[s, hardness];
        }
    }
}
