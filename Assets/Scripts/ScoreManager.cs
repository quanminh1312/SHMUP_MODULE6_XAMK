using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int currentMultiplier = 1;
    private void Start()
    {
        if (instance)
        {
            Debug.LogError("Multiple ScoreManager instances");
            Destroy(gameObject);
            return;
        }
        instance = this;
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
}
