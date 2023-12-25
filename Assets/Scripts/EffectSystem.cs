using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSystem : MonoBehaviour
{
    public static EffectSystem instance;
    public GameObject craftExplosionPrefab = null;
    public GameObject craftDebrisPrefab = null;
    public GameObject craftParticlesPrefab = null;
    void Start()
    {
        if (instance)
        {
            Debug.Log("more than 1 effect system");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }
    public void CraftExplosion(Vector3 position)
    {
        Instantiate(craftExplosionPrefab,position,Quaternion.identity);
        Instantiate(craftDebrisPrefab,position,Quaternion.identity);
        Instantiate(craftParticlesPrefab,position,Quaternion.identity);
    }
}
