using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int currentMultiplier = 1;


    public string[,] names = new string[8, 4];
    public int[,] scores = new int[8, 4];

    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple ScoreManager instances");
            Destroy(gameObject);
            return;
        }
        instance = this;

        for (int h = 0; h < 4; h++)
        {
            for (int s = 0; s < 8; s++)
            {
                names[s, h] = "";
                scores[s, h] = 0;
            }
        }

        LoadScores();
    }
    public void ShootableHit(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void ShootableDestroyed(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void BossDestroyed(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void PickUpCollected(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void BulletDestroyed(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void MedalCollected(int playerIndex, int score)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        GameManager.Instance.playerCrafts[playerIndex].InceaseScore(score * currentMultiplier);
    }
    public void UpdateChainMultiplier(int playerIndex)
    {
        if (!GameManager.Instance.playerCrafts[playerIndex]) return;
        int chain = GameManager.Instance.playerDatas[playerIndex].chain;
        currentMultiplier = (int)Mathf.Pow((chain + 1), 1.5f);
    }
    public void AddScore(int score, int hardness, string name)
    {
        for (int i = 0; i < 8; i++)
        {
            if (scores[i, hardness] < score) //instert here
            {
                ShuffleScoresDown(i,hardness);
                scores[i, hardness] = score;
                names[i, hardness] = name;
                return;
            }
        }
    }
    private void ShuffleScoresDown(int index, int hardness)
    {
        for (int i = 7; i > index; i--)
        {
            scores[i, hardness] = scores[i - 1, hardness];
            names[i, hardness] = names[i - 1, hardness];
        }
    }
    public bool IsTopScore(int score, int hardness)
    {
        if (score > scores[0, hardness])
            return true;
        return false;
    }
    public bool IsHighScore(int score, int hardness)
    {
        for (int s=7; s>=0; s--)
        {
            if (score > scores[s, hardness])
                return true;
        }
        return false;
    }
    public void SaveScore()
    {
        string savePath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("Saving scores to " + savePath);

        try
        {
            FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate);
            BinaryWriter writer = new BinaryWriter(fileStream);
            if (writer != null)
            {
                for (int h = 0; h < 4; h++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        writer.Write(names[s, h]);
                        writer.Write(scores[s, h]);
                    }
                }
            }
            else
                Debug.LogError("failed to create Binary writer for saving hiscores");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Couldn't save scores: " + e.Message);
        }
    }
    public void LoadScores()
    {
        string loadPath = Application.persistentDataPath + "/scrs.dat";
        Debug.Log("Loading scores from " + loadPath);


        try
        {
            FileStream fileStream = new FileStream(loadPath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fileStream);
            if (reader != null)
            {
                for (int h = 0; h < 4; h++)
                {
                    for (int s = 0; s < 8; s++)
                    {
                        names[s, h] = reader.ReadString();
                        scores[s, h] = reader.ReadInt32();
                    }
                }
            }
            else
                Debug.LogError("failed to create Binary reader for loading hiscores");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Couldn't load scores: " + e.Message);
        }
    }
    public int TopScore(int hardness)
    {
        return scores[0, hardness];
    }
}
