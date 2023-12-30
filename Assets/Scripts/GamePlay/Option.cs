using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option : MonoBehaviour
{
    public BulletSpawner shotSpawner = null;

    public void shoot()
    {   
        shotSpawner.shoot(1);
    }
}
