using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Craft : MonoBehaviour
{
    CraftData craftData = new CraftData();
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
        GameManager.Instance.playerOneCraft = null;

        yield return null;
    }
}

public class CraftData
{
    public float positionX;
    public float positionY;
}
