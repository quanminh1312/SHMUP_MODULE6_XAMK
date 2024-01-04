using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeltDestruct : MonoBehaviour
{
    public void Destruct()
    {
        Destroy(gameObject);
    }
    private IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
    private void Start()
    {
        StartCoroutine(DestroyAfterTime(5));
    }
}
