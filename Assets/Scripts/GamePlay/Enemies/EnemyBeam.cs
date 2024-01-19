using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 10f;
    private int layerMask = 0;
    public GameObject beamFlash = null;
    private bool firing = false;

    public GameObject endPoint = null;
    private bool charing = false;
    private const int FULL_CHARGE_TIMER = 60;
    private int chargeTimer = FULL_CHARGE_TIMER;
    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Enemy") & ~LayerMask.GetMask("EnemyBullets");
    }
    public void Fire()
    {
        if (!firing)
        {
            firing = true;
            charing = true;
            UpdateBeam();
            gameObject.SetActive(true);
        }
    }
    public void StopFire()
    {
        if (firing)
        {
            firing = false;
            charing = false;
            gameObject.SetActive(false);
            if (beamFlash != null)
                beamFlash.SetActive(false);
        }
    }
    private void FixedUpdate()
    {
        if (firing)
            UpdateBeam();   
    }
    private void UpdateBeam()
    {
        if (!charing)
        {
            int maxColliders = 20;
            Collider2D[] hits = new Collider2D[maxColliders];

            Vector2 center = Vector2.Lerp(transform.position, endPoint.transform.position, 0.5f);
            Vector2 halfSize = new Vector2(beamWidth / 2, (endPoint.transform.position - transform.position).magnitude * 0.5f);
            int noOfHits = Physics2D.OverlapBoxNonAlloc(center, halfSize,transform.eulerAngles.z, hits, layerMask);
            for (int h = 0; h < noOfHits; h++)
            {
                Craft craft = hits[h].GetComponent<Craft>();
                if (craft)
                {
                    craft.Hit();
                }
            }
            //update visual
            lineRenderer.startWidth = beamWidth;
            lineRenderer.endWidth = beamWidth;
        }
        else
        {
            //update visual
            lineRenderer.startWidth = 1;
            lineRenderer.endWidth = 1;

            //update time
            chargeTimer--;
            if (chargeTimer <= 0)
            {
                chargeTimer = FULL_CHARGE_TIMER;
                charing = false;
                if (beamFlash)
                    beamFlash.SetActive(true);
            }
        }
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPoint.transform.position);
    }
}
