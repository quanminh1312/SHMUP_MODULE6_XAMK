using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPattern))]
public class EnemyPatternEditor : Editor
{
    Mesh previewMesh = null;

    float editorDeltaTime = 0;
    float timer = 0;
    double lastTimeSinceStartUp = 0;


    private void OnEnable()
    {
        if (previewMesh == null)
        {
            previewMesh = new Mesh();

            Vector3[] verts = new Vector3[4];
            Vector2[] uvs = new Vector2[4];
            int[] tris = new int[6];

            const float halfSize = 8f;
            verts[0] = new Vector3(-halfSize, halfSize);
            verts[1] = new Vector3(halfSize, halfSize);
            verts[2] = new Vector3(-halfSize, -halfSize);
            verts[3] = new Vector3(halfSize, -halfSize);
            uvs[0] = new Vector2(0, 1);
            uvs[1] = new Vector2(1, 1);
            uvs[2] = new Vector2(0, 0);
            uvs[3] = new Vector2(1, 0);
            tris[0] = 0;
            tris[1] = 1;
            tris[2] = 2;
            tris[3] = 2;
            tris[4] = 1;
            tris[5] = 3;

            previewMesh.vertices = verts;
            previewMesh.uv = uvs;
            previewMesh.triangles = tris;
        }
    }
    private void OnSceneGUI()
    {
        UpdateEditorTime();
        EnemyPattern pattern = (EnemyPattern)target;
        if (pattern == null ) return;
        UpdatePreView(pattern);
        ProcessInput(pattern);
        if (Event.current.type == EventType.Repaint)
            SceneView.RepaintAll();
    }
    void UpdateEditorTime()
    {
        if (lastTimeSinceStartUp == 0) 
            lastTimeSinceStartUp = EditorApplication.timeSinceStartup;

        editorDeltaTime = (float)(EditorApplication.timeSinceStartup - lastTimeSinceStartUp) * 60f;
        lastTimeSinceStartUp = EditorApplication.timeSinceStartup;
    }
    void ProcessInput(EnemyPattern pattern)
    {
        Event guiEvent = Event.current;
        Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Spline path = pattern.steps[0].spline;
            Vector2 offset = pattern.transform.position;
            path.AddSegment(mousePos - offset);
            path.CalculatePoints(pattern.steps[0].movementSpeed);
        }
    }
    void UpdatePreView(EnemyPattern pattern)
    {
        Vector2 endOfLastStep = pattern.transform.position;
        foreach(EnemyStep step in pattern.steps)
        {
            switch(step.movement)
            {
                case EnemyStep.MoveMentType.direction:
                    Handles.DrawDottedLine(endOfLastStep, endOfLastStep + step.direction, 5);
                    endOfLastStep += step.direction;
                    break;
                case EnemyStep.MoveMentType.spline:
                    endOfLastStep = DrawnSpline(step.spline,endOfLastStep,step.movementSpeed);
                    break;
                case EnemyStep.MoveMentType.homing:
                    if (GameManager.Instance && GameManager.Instance.playerCrafts[0])//error
                    {
                        Handles.DrawDottedLine(endOfLastStep, GameManager.Instance.playerCrafts[0].transform.position, 1);//error
                        endOfLastStep = GameManager.Instance.playerCrafts[0].transform.position;//error
                    }
                    break;
            }
        }
        // draw animated preview
        timer += editorDeltaTime;
        if (timer >= pattern.TotalTime())
            timer = 0;

        SpriteRenderer render = pattern.enmyPrefab.GetComponentInChildren<SpriteRenderer>(); 
        if (render)
        {
            Texture texture = render.sprite.texture;
            Material mat = render.sharedMaterial;

            Vector3 pos = pattern.CalculatePosition(timer);
            Matrix4x4 transMat = Matrix4x4.Translate(pos);
            Quaternion rot = pattern.CalculateRotation(timer);
            Matrix4x4 rotMat = Matrix4x4.Rotate(rot);
            Matrix4x4 matrix = transMat * rotMat;

            mat.SetPass(0);
            Graphics.DrawMeshNow(previewMesh, matrix);

        }
    }
    Vector2 DrawnSpline(Spline spline, Vector2 endoflaststep, float speed)
    {

        //draw control lines
        Handles.color = Color.black;
        for (int s = 0; s < spline.noOfSegments(); s++)
        {
            Vector2[] points = spline.GetSegmentPoints(s,endoflaststep);
            Handles.DrawLine(points[0], points[1]);
            Handles.DrawLine(points[2], points[3]);
        }

        // draw spline line
        Handles.color = Color.white;
        for(int p=1;p<spline.linearPoints.Count;p++)
        {
            Handles.CylinderHandleCap(0, spline.linearPoints[p] + endoflaststep, Quaternion.identity, 0.5f, EventType.Repaint);
        }

        //draw control handles
        Handles.color = Color.green;
        for (int point = 0; point < spline.controlPoints.Count; point++)
        {
            Vector2 pos = spline.controlPoints[point] + endoflaststep;
            float size = 1f;
            if (point % 3 == 0) size = 2f;
            Vector2 newpos = Handles.FreeMoveHandle(pos, size,Vector2.zero, Handles.CylinderHandleCap);
            if (point >0 && pos != newpos)
            {
                spline.SetPoint(point, newpos - endoflaststep);
                spline.CalculatePoints(speed);
            }
        }

        Handles.color = Color.white;

        return endoflaststep;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyPattern pattern = (EnemyPattern)target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Stationary"))
        {
            pattern.AddStep(EnemyStep.MoveMentType.none);
        }
        if (GUILayout.Button("Add Directional"))
        {
            pattern.AddStep(EnemyStep.MoveMentType.direction);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Homing"))
        {
            pattern.AddStep(EnemyStep.MoveMentType.homing);
        }
        if (GUILayout.Button("Add Spline"))
        {
            pattern.AddStep(EnemyStep.MoveMentType.spline);
        }
        GUILayout.EndHorizontal();
    }
}
