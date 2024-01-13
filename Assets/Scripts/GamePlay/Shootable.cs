using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    
    public int health = 10;
    public float radiousOrWidth = 10f;
    public float heigth = 10;
    public bool box = false;

    private Vector2 halfExtent;

    private int layerMask = 0;

    public bool damagedByBullets = true;
    public bool damagedByBombs = true;
    public bool damagedByBeam = true;

    public bool spawnCyclicPickUp = false;
    public PickUp[] spawnSpecificPickUp;
    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") & ~LayerMask.GetMask("EnemyBullets") & ~LayerMask.GetMask("GroundEnemy") ;
        halfExtent = new Vector3(radiousOrWidth/2, heigth/2,0);
    }
    private void FixedUpdate()
    {
        int maxColliders = 10;
        Collider[] hits = new Collider[maxColliders];
        int noOfHits = 0;
        if (box)
            noOfHits = Physics.OverlapBoxNonAlloc(transform.position, halfExtent, hits,transform.rotation, layerMask);
        else
            noOfHits = Physics.OverlapSphereNonAlloc(transform.position, radiousOrWidth, hits, layerMask);
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
                    }
                }
                if (damagedByBombs)
                {
                    Bomb bomb = hits[h].GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        takeDamage(bomb.power,bomb.playerIndex);
                    }
                }
            }
        }
    }
    public void takeDamage(int damage, int fromPlayer)
    {
        health -= damage;
        if (health <= 0)
        {
            if (fromPlayer<2)
            {
                GameManager.Instance.playerDatas[fromPlayer].chain++;
                GameManager.Instance.playerDatas[fromPlayer].chainTimer = PlayerData.MAX_CHAIN;
            }
            Vector2 pos = transform.position;
            if (spawnCyclicPickUp)
            {
                PickUp spawn = GameManager.Instance.GetNextDrop();
                PickUp p = Instantiate(spawn, pos, Quaternion.identity);
                if (p)
                {
                    p.transform.SetParent(GameManager.Instance.transform);
                }
            }
            foreach (PickUp p in spawnSpecificPickUp)
            {
                PickUp pickUp = Instantiate(p, pos, Quaternion.identity);
                if (pickUp)
                {
                    pickUp.transform.SetParent(GameManager.Instance.transform);
                }
            }

            Destroy(gameObject);
        }
    }
}
