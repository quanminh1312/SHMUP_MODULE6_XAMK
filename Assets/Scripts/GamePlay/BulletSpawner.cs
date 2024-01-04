using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public BulletManager.BulletType bulletType = BulletManager.BulletType.bullet1_Size1;

    public BulletSequence sequence = new BulletSequence();

    public int rate = 1;
    public int speed = 10;
    public int timer = 0;

    public GameObject muzzleFlash = null;

    public bool autoFireActive = false;

    public bool firing = false;
    private int frame = 0;  


    public float startAngle = 0;
    public float endAngle = 0;
    public int radialNumber = 1;
    public bool FireAtPlayer = false;
    public bool FireAtTarget = false;
    public GameObject target = null;
    public bool homing = false;
    public float dAngle = 0;

    public bool isPlayer = false;
    public void shoot(int size)
    {
        if (size < 0) return;

        if (!isPlayer)
        {
            float y =transform.position.y;
            if (GameManager.Instance && GameManager.Instance.progressWindow)
                y -= GameManager.Instance.progressWindow.data.positionY;
            if (y<-100 || y>180) 
                return;
        }


        Vector2 primaryDirection = transform.up;
        if (FireAtTarget || (FireAtPlayer && GameManager.Instance && GameManager.Instance.playerOneCraft))
        {
            Vector2 targetPosition = Vector2.zero;
            if (FireAtPlayer && GameManager.Instance && GameManager.Instance.playerOneCraft)
            {
                targetPosition = GameManager.Instance.playerOneCraft.transform.position;
            } else if (FireAtTarget && target)
            {
                targetPosition = target.transform.position;
            }
            primaryDirection = targetPosition - (Vector2) transform.position;
            primaryDirection.Normalize();
        }
        if (firing || timer == 0)
        {
            float angle = startAngle;
            for (int a = 0; a < radialNumber; a++)
            {
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Vector3 velocity = rotation * primaryDirection * speed;
                BulletManager.BulletType bulletToShoot = bulletType + (size - 1);
                GameManager.Instance.bulletManager.SpawnBullet(bulletToShoot,
                                                    transform.position.x,
                                                    transform.position.y,
                                                    velocity.x,
                                                    velocity.y,
                                                    angle,dAngle,homing);
                angle = angle + ((endAngle - startAngle) / (radialNumber - 1));
            }
            if (muzzleFlash)
                muzzleFlash.SetActive(true);
        }
    }
    private void FixedUpdate()
    {
        timer++;
        if (timer > rate)
        {
            timer = 0;
            if (muzzleFlash)
                muzzleFlash.SetActive(false);
            if (autoFireActive)
            {
                firing = true;
                frame = 0;
            }   
        }
        if (firing)
        {
            if (sequence.ShouldFire(frame))
            {
                shoot(1);
            }
            frame++;
            if (frame > sequence.totalFrames)
            {
                firing = false;
            }
        }
    }
    public void Activate()
    {
        autoFireActive = true;
        timer = 0;
        frame = 0;
        firing = true;
    }
    public void Deactivate()
    {
        autoFireActive = false;
    }
}
[Serializable]
public class BulletSequence
{
    public List<int> emitFrames= new List<int>();
    public int totalFrames;

    public bool ShouldFire(int currentFrame)
    {
        foreach (int emitTime in emitFrames)
        {
            if (emitTime == currentFrame) return true;
        }
        return false;
    }
}
