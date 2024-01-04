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
    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") & ~LayerMask.GetMask("EnemyBullets");
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
                        takeDamage(1);
                        GameManager.Instance.bulletManager.DeActivateBullet(b.index);
                    }
                }
                if (damagedByBombs)
                {
                    Bomb bomb = hits[h].GetComponent<Bomb>();
                    if (bomb != null)
                    {
                        takeDamage(bomb.power);
                    }
                }
            }
        }
    }
    public void takeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
