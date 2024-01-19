using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgress : MonoBehaviour
{
    public ProgressData data;
    public int levelSize;
    public AnimationCurve speedCurve = new AnimationCurve();
    public GameObject background = null;

    public GameObject midGroundTitleGrid;
    public float midGroundRate = 0.75f;

    private Craft playerOneCraft = null;

    private float ratio;
    // Start is called before the first frame update
    void Start()
    {
        data.positionX = transform.position.x;
        data.positionY = transform.position.y;

        if (GameManager.Instance)
        {
            GameManager.Instance.progressWindow = this;
        }
        ratio = (float)data.progress / (float)levelSize;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (data.progress < levelSize)
        {
            float movement = speedCurve.Evaluate(ratio);
            data.progress = (int)(data.positionY + movement);

            if (playerOneCraft==null)
                playerOneCraft = GameManager.Instance.playerCrafts[0];//error
            if (playerOneCraft)
                UpdateProgressWindow(playerOneCraft.craftData.positionX,movement);
        }
    }
    void UpdateProgressWindow(float shipX,float movement)
    {
        data.positionX = shipX/10f;
        data.positionY += movement;
        transform.position = new Vector3(data.positionX, data.positionY, transform.position.z);
        float z = background.transform.position.z;
        background.transform.position = new Vector3(data.positionX, data.positionY, z);
        if (midGroundTitleGrid)
            midGroundTitleGrid.transform.position = new Vector3(0, data.positionY * midGroundRate,0);
    }
}

[Serializable]
public class ProgressData
{
    public int progress;
    public float positionX;
    public float positionY;
}
