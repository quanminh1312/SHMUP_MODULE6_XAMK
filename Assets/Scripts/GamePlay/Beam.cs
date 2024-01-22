using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Beam : MonoBehaviour
{
    public LineRenderer lineRenderer = null;
    public float beamWidth = 20f;
    public Craft craft = null;
    public int playerIndex = 2;
    private int layerMask = 0;
    public GameObject beamFlash = null;
    public GameObject[] beamHits = new GameObject[5];
    const int MINIMUM_CHARGE = 10;
    const int MAXRAYHITS = 10;

    public AudioSource audioSource = null;


    CraftData craftData;

    private void Start()
    {
        layerMask = ~LayerMask.GetMask("Player") & ~LayerMask.GetMask("PlayerBullets");
    }
    public void Fire()
    {
        if (craftData == null) craftData = GameManager.Instance.gameSession.craftDatas[craft.playerIndex];
        if (!craftData.beamFiring)
        {
            if (craftData.beamCharge < MINIMUM_CHARGE)
            {
                UpdateBeam();
                return;
            }
            craftData.beamFiring = true;
            craftData.beamTimer = craftData.beamCharge;
            craftData.beamCharge = 0;
            UpdateBeam();
            gameObject.SetActive(true);
            beamFlash.SetActive(true);
            if (audioSource)
            {
                audioSource.Play();
            }
        }
    }
    private void FixedUpdate()
    {
        if (craftData == null) craftData = GameManager.Instance.gameSession.craftDatas[craft.playerIndex];
        if (craftData.beamFiring)
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
        if (craftData == null) craftData = GameManager.Instance.gameSession.craftDatas[craft.playerIndex];
        if (craftData.beamTimer > 0 ) craftData.beamTimer--;
        if (craftData.beamTimer <= 0)
        {
            craftData.beamFiring = false;
            HideHits();
            if (audioSource)
            {
                audioSource.Stop();
            }
            gameObject.SetActive(false);
            beamFlash.SetActive(false);
        }
        else
        {
            float scale = beamWidth / 30f;
            beamFlash.transform.localScale = new Vector3(scale, scale, 1);

            float topY = 180;
            //if (GameManager.Instance && GameManager.Instance.progressWindow)
            //    topY += GameManager.Instance.progressWindow.data.positionY;
            topY += craft.transform.position.y;

            int maxColliders = 20;
            Collider2D[] hits = new Collider2D[maxColliders];
            Vector2 halfSize = new Vector2(beamWidth * 0.5f, (topY - craft.transform.position.y) * 0.5f);
            float midlleY = (craft.transform.position.y + topY) * 0.5f;
            Vector3 center = new Vector3(craft.transform.position.x, midlleY, 0);
            int noOfHits = Physics2D.OverlapBoxNonAlloc(center
                                                        , halfSize
                                                        , 0 //transfrom.rotation,
                                                        , hits
                                                        , layerMask);
            float lowest = topY;
            Shootable lowestShootable = null;
            Collider2D lowestCollider = null;
            bool first = false;
            if (noOfHits > 0)
            {
                //find lowest hit
                for (int h = 0; h < noOfHits; h++)
                {
                    Shootable shootable = hits[h].GetComponent<Shootable>();
                    if (shootable && shootable.damagedByBeam)
                    {
                        //FindLowHit(topY, ref first, ref lowest, shootable, hits[h], hits, h);
                        if (hits[h].transform.position.y < lowest || !first)
                        {
                            first = true;
                            lowest = hits[h].transform.position.y;
                            lowestShootable = hits[h].GetComponent<Shootable>();
                            lowestCollider = hits[h];
                        }
                    }
                }
                //find hits on colliders
                if (lowestShootable)
                {
                    lowestShootable.takeDamage(craftData.beamPower + 1, playerIndex);
                    Vector3 start = craft.transform.position;
                    start.x -= (beamWidth / 5);
                    //fire 5 rays to find each hit
                    for (int h = 0; h < 5; h++)
                    {
                        //Fire5rays(lowestCollider, ref start, h);
                        RaycastHit2D hit = Physics2D.Raycast(start, Vector2.up, 360);
                        if (hit.collider == lowestCollider)
                        {
                            Vector3 pos = hit.point;
                            pos.x += Random.Range(-3f, 3f);
                            pos.y += Random.Range(-3f, 3f);
                            beamHits[h].transform.position = pos;
                            beamHits[h].gameObject.SetActive(true);
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
    public void FindLowHit(float topY, ref bool first, ref float lowest, Shootable lowestShootable, Collider2D lowestCollider, Collider2D[] hits, int h)
    {
        RaycastHit2D[] hitInfo = new RaycastHit2D[MAXRAYHITS];
        Vector2 ray = Vector3.up;
        float heigth = topY - craft.transform.position.y;
        if (hits[h].Raycast(ray, hitInfo, heigth) >= 0)
        {
            if (hitInfo[0].point.y < lowest || !first)
            {
                first = true;
                lowest = hitInfo[0].point.y;
                lowestShootable = hits[h].GetComponent<Shootable>();
                lowestCollider = hits[h];
            }
        }
    }
    public void Fire5rays(Collider2D lowestCollider, ref Vector3 start, int h)
    {
        RaycastHit2D[] hitInfo = new RaycastHit2D[MAXRAYHITS];
        Vector2 ray = Vector3.up;
        if (lowestCollider.Raycast(ray, hitInfo, 360) > 0)
        {
            Vector3 pos = hitInfo[0].point;
            pos.x += Random.Range(-3f, 3f);
            pos.y += Random.Range(-3f, 3f);
            beamHits[h].transform.position = pos;
            beamHits[h].gameObject.SetActive(true);
        }
        else
        {
            beamHits[h].gameObject.SetActive(false);
        }
        start.x += (beamWidth / 5);
    }
}
