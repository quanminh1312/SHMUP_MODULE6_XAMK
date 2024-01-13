using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{
    public CraftData craftData = new CraftData();
    Vector3 newPosition = new Vector3();

    public SpriteRenderer flame1;
    public SpriteRenderer flame2;
    public SpriteRenderer miniflame1;
    public SpriteRenderer miniflame2;

    public CraftConfiguration config;
    public int playerIndex;

    Animator animator;
    int leftBoolID;
    int rightBoolID;

    SpriteRenderer spriteRender = null;
    bool alive = true;
    bool invunerable = true;
    int invunerableTimer = 120;
    const int INVUNERABLELENGHT = 120;
    public static int MAXIMUMBEAMCHARGE = 64;

    int layerMask = 1;
    int pickUpLayer = 0;

    public BulletSpawner[] bulletSpawner = new BulletSpawner[5];
    public Option[] options = new Option[4] ;

    public GameObject[] optionMarker1 = new GameObject[4];
    public GameObject[] optionMarker2 = new GameObject[4];
    public GameObject[] optionMarker3 = new GameObject[4];
    public GameObject[] optionMarker4 = new GameObject[4];

    public Beam beam = null;

    public GameObject BombPrefab = null;
    private void Start()
    {
        animator = GetComponent<Animator>();
        Debug.Assert(animator);
        leftBoolID = Animator.StringToHash("left");
        rightBoolID = Animator.StringToHash("right");

        flame1.enabled = false; 
        flame2.enabled = false;

        spriteRender = GetComponent<SpriteRenderer>();  
        Debug.Assert(spriteRender);

        layerMask = ~LayerMask.GetMask("PlayerBullets") & ~LayerMask.GetMask("Player") 
                  & ~LayerMask.GetMask("GroundEnemy");
        pickUpLayer = LayerMask.NameToLayer("PickUp");
    }
    public void SetInvunerable()
    {
        invunerable = true;
        invunerableTimer = INVUNERABLELENGHT;
    }
    private void FixedUpdate()
    {

        if (invunerable)
        {
            if (invunerableTimer % 12 < 6)
                spriteRender.material.SetColor("_OverBright",Color.black);
            else
                spriteRender.material.SetColor("_OverBright", Color.white);
            invunerableTimer--;
            if (invunerableTimer < 0)
            {
                invunerable = false;
                spriteRender.material.SetColor("_OverBright", Color.black);
            }
        }

        // hit detection
        int maxColliders = 10;
        Collider[] hits = new Collider[maxColliders];


        //bullet hits
        Vector2 halfSize = new Vector2(3f, 4f);
        int noOfHits = Physics.OverlapBoxNonAlloc(transform.position
                                    , halfSize
                                    , hits,Quaternion.identity,layerMask);
        if (noOfHits > 0)
        {
            foreach (Collider hit in hits)
            {
                if (hit)
                {
                    if (hit.gameObject.layer != pickUpLayer) Hit();
                }
            }
        }


        // pickups and Bullet Grazing
        halfSize = new Vector2(15f, 20f);
        noOfHits = Physics.OverlapBoxNonAlloc(transform.position
                                    , halfSize
                                    , hits, Quaternion.identity, layerMask);
        if (noOfHits > 0)
        {
            foreach (Collider hit in hits)
            {
                if (hit)
                {
                    if (hit.gameObject.layer == pickUpLayer) PickUp(hit.GetComponent<PickUp>());
                    else //bullet grazing
                    if (craftData.beamCharge < MAXIMUMBEAMCHARGE)
                    {
                        craftData.beamCharge++;
                        craftData.beamTimer++;
                    }
                }
            }
        }


        //movement
        if (InputManager.instance && alive)
        {
            // Chain drop
            if (GameManager.Instance.playerDatas[playerIndex].chainTimer > 0)
            {
                GameManager.Instance.playerDatas[playerIndex].chainTimer--;
                if (GameManager.Instance.playerDatas[playerIndex].chainTimer == 0)
                {
                    GameManager.Instance.playerDatas[playerIndex].chain = 0;
                }
            }

            //movement
            craftData.positionX += InputManager.instance.playerState[0].movement.x * config.speed;
            craftData.positionY += InputManager.instance.playerState[0].movement.y * config.speed;

            if (craftData.positionX<-146 + halfSize.x) craftData.positionX = -146 + halfSize.x;
            if (craftData.positionX>146 - halfSize.x) craftData.positionX = 146 - halfSize.x;

            if (craftData.positionY < -180 + halfSize.y) craftData.positionY = -180 + halfSize.y;
            if (craftData.positionY > 180 - halfSize.y) craftData.positionY = 180 - halfSize.y;

            newPosition.x = (int) craftData.positionX;
            if (!GameManager.Instance.progressWindow)
                GameManager.Instance.progressWindow = FindObjectOfType<LevelProgress>();
            if (GameManager.Instance.progressWindow)
                newPosition.y = (int)craftData.positionY + GameManager.Instance.progressWindow.transform.position.y;
            else
                newPosition.y = (int)craftData.positionY;
            gameObject.transform.position = newPosition;
            CheckUp();
            if (InputManager.instance.playerState[0].left)
            {
                AllTrue();
                animator.SetBool(leftBoolID,true);
            }
            else
            {
                animator.SetBool(leftBoolID, false);
            }
            if (InputManager.instance.playerState[0].right)
            {
                AllTrue();
                animator.SetBool(rightBoolID, true);
            }
            else
            {
                animator.SetBool(rightBoolID, false);
            }
            if (InputManager.instance.playerState[0].shoot)
            {
                ShotConfiguration shotConfig = config.shotlevel[craftData.shotPower];
                for (int s=0;s<5;s++)
                {
                    bulletSpawner[s].shoot(shotConfig.spawnerSizes[s]);
                }

                for (int o=0;o<craftData.noOfEnableOptions;o++)
                {
                    if (options[o])
                    {
                        options[o].shoot();
                    }
                }
            }
            if (InputManager.instance.playerState[0].option && !InputManager.instance.prePlayerState[0].option)
            {
                craftData.optionsLayout++;
                if (craftData.optionsLayout > 3) craftData.optionsLayout = (char)0;
                SetOptionLayout(craftData.optionsLayout);
            }
            if (InputManager.instance.playerState[0].beam && !InputManager.instance.prePlayerState[0].beam)
            {
                beam.Fire();
            }
            if (InputManager.instance.playerState[0].bomb && !InputManager.instance.prePlayerState[0].bomb)
            {
                FireBomb();
            }
        }
    }
    public void PickUp(PickUp pickUp)
    {
        if (pickUp)
        {
            pickUp.ProcessPickUp(playerIndex, craftData);
        }
    }
    public void PowerUp(int powerLevel)
    {
        craftData.shotPower += powerLevel;
        if (craftData.shotPower > CraftConfiguration.MAX_SHOT_POWER - 1)
            craftData.shotPower = CraftConfiguration.MAX_SHOT_POWER - 1;
    }
    public void Hit()
    {
        if (!invunerable)
        {
            Explode();
        }
    }
    void FireBomb()
    {
        if (craftData.smallBombs >0)
        {
            craftData.smallBombs--;
            Vector3 pos = transform.position;
            pos.y += 100;
            Bomb bomb = Instantiate(BombPrefab, pos, Quaternion.identity).GetComponent<Bomb>();
            if (bomb) bomb.playerIndex = playerIndex;
        }
    }
    private void CheckUp()
    {
        if (InputManager.instance.playerState[0].up)
        {
            flame1.enabled = true;
            flame2.enabled = true;
            miniflame1.enabled = false;
            miniflame2.enabled = false;
        }
        else
        {
            flame1.enabled = false;
            flame2.enabled = false;
            miniflame1.enabled = true;
            miniflame2.enabled = true;
        }
    }
    private void AllTrue()
    {
        flame1.enabled = true;
        flame2.enabled = true;
        miniflame1.enabled = true;
        miniflame2.enabled = true;
    }
    public void Explode()
    {
        alive = false;
        StartCoroutine(Exploding());
    }

    IEnumerator Exploding()
    {
        Color color= Color.white;
        for (float redness = 0; redness <=1; redness+=0.3f)
        {
            color.g = 1 - redness;
            color.b = 1 - redness;
            spriteRender.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        
        EffectSystem.instance.CraftExplosion(transform.position);
        Destroy(gameObject);
        GameManager.Instance.playerCrafts[0] = null;//error

        yield return null;
    }
    public void AddOption()
    {
        if (craftData.noOfEnableOptions < 4)
        {
            options[craftData.noOfEnableOptions].gameObject.SetActive(true);
            craftData.noOfEnableOptions++;
        }
    }
    public void SetOptionLayout(int layoutIndex)
    {
        Debug.Assert(layoutIndex < 4);

        for(int o=0;o<4;o++)
        {
            switch(layoutIndex)
            {
                case 0:
                    options[o].gameObject.transform.position = optionMarker1[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarker1[o].transform.rotation;
                    break;
                case 1:
                    options[o].gameObject.transform.position = optionMarker2[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarker2[o].transform.rotation;
                    break;
                case 2:
                    options[o].gameObject.transform.position = optionMarker3[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarker3[o].transform.rotation;
                    break;
                case 3:
                    options[o].gameObject.transform.position = optionMarker4[o].transform.position;
                    options[o].gameObject.transform.rotation = optionMarker4[o].transform.rotation;
                    break;
            }
        }
    }
    public void IncreaseBeamStrenght()
    {
        if(craftData.beamPower < 5)
        {
            craftData.beamPower++;
            UpdateBeam();
        }
    }
    public void AddBomb(int power)
    {
        if (power == 1)
            craftData.smallBombs++;
        else if (power == 2)
            craftData.largeBombs++;
        else
            Debug.LogError("Invalid bomb power pickup");
    }
    public void OneUp()
    {
        GameManager.Instance.playerDatas[playerIndex].lives++;
    }
    public void AddMedal(int value, int level)
    {
        InceaseScore(value);
    }
    void UpdateBeam()
    {
        beam.beamWidth = (craftData.beamPower + 2) * 8f;
    }
    public void InceaseScore(int score)
    {
        GameManager.Instance.playerDatas[playerIndex].score += score;
        GameManager.Instance.playerDatas[playerIndex].stageScore += score;
    }
}
[Serializable]
public class CraftData
{
    public float positionX;
    public float positionY;

    public int shotPower;

    public int noOfEnableOptions;
    public int optionsLayout;

    public bool beamFiring;
    public int beamPower; //power of beam abnd width
    public int beamCharge; // picked by charge
    public int beamTimer; //current charge

    public int smallBombs;
    public int largeBombs;
}
