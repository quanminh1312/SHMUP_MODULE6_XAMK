using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 20f;
    public Craft craft = null;
    private int layerMask = 0;
    public GameObject beamFlash = null;
    public GameObject[] beamHits = new GameObject[5];
    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("PlayerBullets");
    }
    public void Fire()
    {
        if (!craft.craftData.beamFiring)
        {
            craft.craftData.beamFiring = true;
            craft.craftData.beamTimer = craft.craftData.beamCharge;
            UpdateBeam();
            float scale = beamWidth/30f;
            beamFlash.transform.localScale = new Vector3(scale, scale, 1);
            gameObject.SetActive(true);
            beamFlash.SetActive(true);
        }
    }
    private void FixedUpdate()
    {
        UpdateBeam();   
    }
    void HideHits()
    {
        for (int h = 0; h < 5; h++)
        {
            beamHits[h].gameObject.SetActive(false);
        }
    }
    private void UpdateBeam()
    {
        craft.craftData.beamTimer--;
        if (craft.craftData.beamTimer <= 0)
        {
            craft.craftData.beamFiring = false;
            HideHits();
            gameObject.SetActive(false);
            beamFlash.SetActive(false);
        }
        else
        {
            int maxColliders = 20;
            Collider[] hits = new Collider[maxColliders];
            Vector2 halfSize = new Vector2(beamWidth * 0.5f, (180 - craft.transform.position.y) * 0.5f);
            float midlleY = (craft.transform.position.y + 180) * 0.5f;
            Vector3 center = new Vector3(craft.transform.position.x, midlleY, 0);
            int noOfHits = Physics.OverlapBoxNonAlloc(center, halfSize, hits, Quaternion.identity, layerMask);
            float lowest = 180;
            Shootable lowestShootable = null;
            Collider lowestCollider = null;
            if (noOfHits > 0)
            {
                //find lowest hit
                for (int h = 0; h < noOfHits; h++)
                {
                    RaycastHit hitInfo;
                    Ray ray = new Ray(craft.transform.position, Vector3.up);
                    float heigth = 180 - craft.transform.position.y;
                    if (hits[h].Raycast(ray, out hitInfo, heigth))
                    {
                        if (hitInfo.point.y < lowest)
                        {
                            lowest = hitInfo.point.y;
                            lowestShootable = hits[h].GetComponent<Shootable>();
                            lowestCollider = hits[h];
                        }
                    }
                }

                //find hits on colliders
                if (lowestShootable)
                {
                    Vector3 start = craft.transform.position;
                    start.x -= (beamWidth / 5);
                    //fire 5 rays to find each hit
                    for (int h = 0; h < 5; h++)
                    {
                        RaycastHit hitInfo;
                        Ray ray = new Ray(start, Vector3.up);
                        if (lowestCollider.Raycast(ray, out hitInfo, 360))
                        {
                            Vector3 pos = hitInfo.point;
                            pos.x += Random.Range(-3f, 3f);
                            pos.y += Random.Range(-3f, 3f);
                            beamHits[h].transform.position = pos;
                            beamHits[h].gameObject.SetActive(true);
                            lowestShootable.takeDamage(craft.craftData.beamPower + 1);
                        }
                        else
                        {
                            beamHits[h].gameObject.SetActive(false);
                        }
                        start.x += (beamWidth / 5);
                    }
                }
                else
                {
                    HideHits();
                }
            }
            else
            {
                HideHits();
            }


            //update visual
            lineRenderer.startWidth = beamWidth;
            lineRenderer.endWidth = beamWidth;


            lineRenderer.SetPosition(0, transform.position);
            Vector3 top = transform.position;
            top.y = lowest;
            lineRenderer.SetPosition(1, top);
        }
    }
}
