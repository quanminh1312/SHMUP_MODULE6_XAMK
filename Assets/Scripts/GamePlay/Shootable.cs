using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    
    public int health = 10;
    public float radiousOrWidth = 10f;
    public float heigth = 10;
    public bool box = false;
    public bool polygon = false;

    public bool remainDestroy = false;
    private bool destroyed = false;
    public int damageHealth = 5; //at what health is damage sprite displayed

    private Collider2D polyCollider;

    private Vector2 halfExtent;

    private int layerMask = 0;

    public bool damagedByBullets = true;
    public bool damagedByBombs = true;
    public bool damagedByBeam = true;

    public bool spawnCyclicPickUp = false;
    public PickUp[] spawnSpecificPickUp;


    public SoundFX destroyedSound = null;

    public int hitScore = 10;
    public int destroyScore = 1000;

    
    private bool flashing = false;
    private float flashTimer = 0;
    private SpriteRenderer spriteRenderer = null;

    public bool largeExplosion = false;
    public bool smallExplosion = false;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") & ~LayerMask.GetMask("EnemyBullets") & ~LayerMask.GetMask("GroundEnemy") ;
        if (polygon)
        {
            polyCollider = GetComponent<Collider2D>();
            Debug.Assert(polyCollider);
        }
        else
            halfExtent = new Vector3(radiousOrWidth/2, heigth/2,0);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void FixedUpdate()
    {
        if (destroyed) return;
        if (flashing)
        {
            flashTimer -= Time.deltaTime;
            if (flashTimer <= 0)
            {
                spriteRenderer.material.SetColor("_OverBright",Color.black);
                flashing = false;
            }
        }
        int maxColliders = 10;
        Collider2D[] hits = new Collider2D[maxColliders];
        int noOfHits = 0;
        if (box)
        {
            float angle = transform.eulerAngles.z;
            noOfHits = Physics2D.OverlapBoxNonAlloc(transform.position,
                                                    halfExtent,
                                                    angle,
                                                    hits,
                                                    layerMask);
        }
        else if (polygon)
        {
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(layerMask);
            contactFilter.useLayerMask = true;
            noOfHits = Physics2D.OverlapCollider(polyCollider, contactFilter, hits);
        }
        else
            noOfHits = Physics2D.OverlapCircleNonAlloc(transform.position, radiousOrWidth, hits, layerMask);
        if (noOfHits >0)
        {
            for (int h=0;h<noOfHits;h++)
            {
                if (damagedByBullets)
                {
                    Bullet b = hits[h].GetComponent<Bullet>();
                    if (b != null)
                    {
                        takeDamage(1,b.playerIndex);
                        GameManager.Instance.bulletManager.DeActivateBullet(b.index);
                        FlashAndSparks(b.transform.position);
                    }
                }
                if (damagedByBombs)
                {
                    Bomb bomb = hits[h].GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        takeDamage(bomb.power,bomb.playerIndex);
                        FlashAndSparks(transform.position);
                    }
                }
            }
        }
    }

    private void FlashAndSparks(Vector3 position)
    {
        EffectSystem.instance.SpawnSparks(position);


        if (flashing) return;
        flashing = true;
        flashTimer = 0.01f;
        spriteRenderer.material.SetColor("_OverBright", Color.white);
    }
    public void takeDamage(int damage, int fromPlayer)
    {
        if (destroyed) return;

        ScoreManager.instance.ShootableHit(fromPlayer, hitScore);
        
        health -= damage;

        EnemyPart part = GetComponent<EnemyPart>();
        if (health <= damageHealth && part)
            part.Damaged(true);
        else if (part)
            part.Damaged(false);


        if (health <= 0) //destroyed
        {
            destroyed = true;
            if (part) part.Destroyed(fromPlayer);

            if (destroyedSound)
                destroyedSound.Play();

            if (fromPlayer<2)
            {
                ScoreManager.instance.ShootableDestroyed(fromPlayer, destroyScore);
                GameManager.Instance.playerDatas[fromPlayer].chain++;
                ScoreManager.instance.UpdateChainMultiplier(fromPlayer);
                GameManager.Instance.playerDatas[fromPlayer].chainTimer = PlayerData.MAX_CHAIN;
            }
            Vector2 pos = transform.position;
            if (spawnCyclicPickUp)
            {
                PickUp spawn = GameManager.Instance.GetNextDrop();
                GameManager.Instance.SpawnPickUp(spawn, pos);
            }
            foreach (PickUp p in spawnSpecificPickUp)
            {
                GameManager.Instance.SpawnPickUp(p, pos);
            }

            if (smallExplosion)
                EffectSystem.instance.SpawnSmallExplosion(transform.position);
            else if (largeExplosion)
                EffectSystem.instance.SpawnLargeExplosion(transform.position);


            if (remainDestroy)
                destroyed = true;
            else
                gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
