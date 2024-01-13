using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedChar : MonoBehaviour
{
    public Sprite[] charSprites;
    private SpriteRenderer spriteRenderer;
    private Image image;

    public int digit = 0;
    private int frame = 0;

    public int offset = 0;

    public int noOfCharacters;
    public int noOfFrames;

    public float FPS = 10f;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
        {
            image = GetComponent<Image>();
            Debug.Assert(image != null);
        }
        else Debug.Assert(spriteRenderer !=null);
        timer = 1f / FPS;
        UpdateSprite(0);
    }
    public void UpdateSprite(int NewFrame)
    {
        int loppedFrame = (NewFrame + offset) % noOfFrames;
        int spriteIndex = loppedFrame + (digit * noOfFrames);
        if (spriteRenderer)
            spriteRenderer.sprite = charSprites[spriteIndex];
        else if (image)
            image.sprite = charSprites[spriteIndex];
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            frame++;
            if (frame >= noOfFrames)
            {
                frame = 0;
            }
            UpdateSprite(frame);
            timer = 1f / FPS;
        }   
    }
}
