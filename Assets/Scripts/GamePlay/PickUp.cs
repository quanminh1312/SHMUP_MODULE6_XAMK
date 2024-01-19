using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public enum PickUpType
    {
        INVALID,
        Bomb,
        Coin,
        PowerUp,
        BeamUp,
        Medal,
        Secret,
        Lives,
        options,
        NOOFPICKUPTYPES
    }
    public PickUpConfig config;
    public Vector2 position;
    public Vector2 velocity;


    public SoundFX sounds = null;

    private void OnEnable()
    {
        position = transform.position;
        velocity.x = Random.Range(-4,4);
        velocity.y = Random.Range(-4, 4);
    }
    private void FixedUpdate()
    {
        //move
        position+= velocity;
        velocity /= 1.3f;

        position.y -= config.fallSpeed;
        if (GameManager.Instance && GameManager.Instance.progressWindow)
        {
            float posY = position.y - GameManager.Instance.progressWindow.transform.position.y;
            if (posY < -180)//off screen
            {
                GameManager.Instance.PickUpFallOffScreen(this);
                Destroy(gameObject);
                return;
            }
        }
        transform.position = position;
    }

    public void ProcessPickUp(int playerIndex, CraftData craftData)
    {
        if (sounds)
        {
            sounds.Play();
        }
        switch (config.type)
        {
            case PickUpType.Coin:
                //GameManager.Instance.playerCrafts[playerIndex].InceaseScore(config.coinValue);
                ScoreManager.instance.PickUpCollected(playerIndex, config.coinValue);
                break;
            case PickUpType.PowerUp:
                GameManager.Instance.playerCrafts[playerIndex].PowerUp(config.powerLevel, config.surplusValue);
                break;
            case PickUpType.Lives:
                GameManager.Instance.playerCrafts[playerIndex].OneUp(config.surplusValue);
                break;
            case PickUpType.Secret:
                ScoreManager.instance.PickUpCollected(playerIndex, config.coinValue);
                //GameManager.Instance.playerCrafts[playerIndex].InceaseScore(config.coinValue);
                break;
            case PickUpType.BeamUp:
                GameManager.Instance.playerCrafts[playerIndex].IncreaseBeamStrenght(config.surplusValue);
                break;
            case PickUpType.options:
                GameManager.Instance.playerCrafts[playerIndex].AddOption(config.surplusValue);
                break;
            case PickUpType.Bomb:
                GameManager.Instance.playerCrafts[playerIndex].AddBomb(config.bombPower,config.surplusValue);
                break;
            case PickUpType.Medal:
                GameManager.Instance.playerCrafts[playerIndex].AddMedal(config.medalValue,config.medalLevel);
                break;
            default:
                Debug.LogError("UnProcessed config type " + config.type);
                break;
        }
        Destroy(gameObject);
    }
}
