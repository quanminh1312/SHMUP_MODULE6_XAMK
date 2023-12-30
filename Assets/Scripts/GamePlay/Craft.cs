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

    int layerMask = 1;

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

        layerMask = ~LayerMask.GetMask("PlayerBullets") & ~LayerMask.GetMask("Player");

        craftData.beamCharge =(char) 100;
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
        Vector2 halfSize = new Vector2(15f,20f);
        Collider[] hits = new Collider[maxColliders];
        int noOfHits = Physics.OverlapBoxNonAlloc(transform.position
                                    , halfSize
                                    , hits,Quaternion.identity,layerMask);
        if (noOfHits > 0)
        {
            if (!invunerable)
            {
                Explode();
            }
        }
        if (InputManager.instance && alive)
        {
            craftData.positionX += InputManager.instance.playerState[0].movement.x * config.speed;
            craftData.positionY += InputManager.instance.playerState[0].movement.y * config.speed;
            newPosition.x = (int) craftData.positionX;
            newPosition.y = (int) craftData.positionY;
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
    void FireBomb()
    {
        Vector3 pos = transform.position;
        pos.y += 100;
        Instantiate(BombPrefab, pos, Quaternion.identity);
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
        GameManager.Instance.playerOneCraft = null;

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
    void UpdateBeam()
    {
        beam.beamWidth = (craftData.beamPower + 2) * 8f;
    }
}

public class CraftData
{
    public float positionX;
    public float positionY;

    public int shotPower;

    public int noOfEnableOptions;
    public char optionsLayout;

    public bool beamFiring;
    public char beamPower; //power of beam abnd width
    public char beamCharge; // max charge(upgradeable)
    public char beamTimer; //current charge
}
