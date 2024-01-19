using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyPart : MonoBehaviour
{
    public bool destroyed = false;
    public bool damaged = false;

    bool usingDamageSprite = false;
    public Sprite damagedVersion = null;
    public Sprite destroyVersion = null;

    public  UnityEvent triggerOnDestroy = null;

    public int destroyByPlayer = 2;

    public void Destroyed(int playerIndex)
    {
        if (destroyed) return;

        destroyByPlayer = playerIndex;

        triggerOnDestroy.Invoke();

        if (destroyVersion)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = destroyVersion;
            }
        }
        destroyed = true;
        Enemy enemy = transform.root.GetComponent<Enemy>();
        if (enemy)
        {
            enemy.PartDestroyed();
        }
    }
    public void Damaged(bool SwitchToDamagedSprite)
    {
        if (destroyed) return;
        if (SwitchToDamagedSprite && damagedVersion && !usingDamageSprite)
        {
            usingDamageSprite = true;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                spriteRenderer.sprite = damagedVersion;
            }
        }

        damaged = true;

    }

}
