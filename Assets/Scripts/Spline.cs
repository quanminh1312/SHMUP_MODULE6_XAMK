using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Spline
{
    [SerializeField]
    public List<Vector2> controlPoints;
    
    [SerializeField]
    bool isClosed;

    [HideInInspector]
    public List<Vector2> linearPoints = new List<Vector2>();

    public Vector2[] GetSegmentPoints(int s, Vector2 offset)
    {
        return new Vector2[] 
        {
              controlPoints[s*3] + offset
            , controlPoints[s * 3 + 1] + offset
            , controlPoints[s * 3 + 2] + offset
            , controlPoints[s * 3 + 3] + offset
        };
    }
    public int noOfSegments()
    {
            return controlPoints.Count/3;
    }
    public Spline()
    {
        controlPoints = new List<Vector2>
        {
            Vector2.zero,
            Vector2.down * 20,
            (Vector2.down + Vector2.right) * 20,
            (Vector2.down + Vector2.right + Vector2.down) * 20
        };
        CalculatePoints(4);
    }
    public void CalculatePoints(float speed)
    {
        linearPoints.Clear();

        linearPoints.Add(controlPoints[0]);
        Vector2 prevPoint = controlPoints[0];
        float disSinceLast = 0;

        for (int s = 0; s < noOfSegments(); s++)
        {
            Vector2[] segmentPoints = GetSegmentPoints(s,Vector2.zero);



            float netLength = Vector2.Distance(segmentPoints[0], segmentPoints[1]) +
                              Vector2.Distance(segmentPoints[1], segmentPoints[2]) +
                              Vector2.Distance(segmentPoints[2], segmentPoints[3]);

            float curveLength = Vector2.Distance(segmentPoints[0], segmentPoints[3]) + (netLength/2);

            float spacing = speed;
            int divisions = Mathf.CeilToInt(curveLength/spacing);
            if (divisions > 1000) divisions = 1000;


            float t = 0;
            while (t <= 1)
            {
                t+=1f/divisions;
                Vector2 pointOnCurver = Bezier.calculateCubic(segmentPoints[0], segmentPoints[1], segmentPoints[2], segmentPoints[3], t);
                
                disSinceLast += Vector2.Distance(prevPoint, pointOnCurver);

                while (disSinceLast >= spacing)
                {
                    float overshoot = disSinceLast - spacing;
                    Vector2 newEvenlySpacedPoint = pointOnCurver + (prevPoint - pointOnCurver).normalized * overshoot;
                    linearPoints.Add(newEvenlySpacedPoint);
                    disSinceLast = overshoot;
                    prevPoint = newEvenlySpacedPoint;
                }
                prevPoint = pointOnCurver;
            }
        }
    }
    public void SetPoint(int index, Vector2 newPosition)
    {
        Vector2 dPos = newPosition - controlPoints[index];
        controlPoints[index] = newPosition;
        if (index % 3 == 0) //anchor point
        {
            if (index + 1 < controlPoints.Count)
            {
                controlPoints[index + 1] += dPos;
            }
            if (index - 1 >= 0)
            {
                controlPoints[index - 1] += dPos;
            }
        }
        else //angle tangent point
        {
            bool nextPointIsAnchor = (index + 1) % 3 == 0;
            int anchorIndex = (nextPointIsAnchor) ? index + 1 : index - 1;
            int controlIndex = (nextPointIsAnchor) ? index + 2 : index - 2;
            if (controlIndex>=0 && controlIndex < controlPoints.Count)
            {
                float dist = (controlPoints[controlIndex] - controlPoints[anchorIndex]).magnitude;
                Vector2 dir = (controlPoints[anchorIndex] - newPosition).normalized;
                controlPoints[controlIndex] = controlPoints[anchorIndex] + dir * dist;
            }
        }
    }
    public void AddSegment(Vector2 position)
    {
        Vector2 point1 = controlPoints[controlPoints.Count - 1] +
                         controlPoints[controlPoints.Count - 1] - 
                         controlPoints[controlPoints.Count - 2];
        Vector2 point2 = (point1 + position) * 0.5f;
        controlPoints.Add(point1);
        controlPoints.Add(point2);
        controlPoints.Add(position);
    }
    public Vector2 GetPosition(float normalisedTime)
    {
        if (normalisedTime < 0)
        {
            Debug.LogWarning("getposition sent - time!");
            return Vector2.zero;
        }
        if (normalisedTime > 1)
        {
            Debug.LogWarning("getposition sent time > 1!");
            return Vector2.zero;
        }
        if (linearPoints.Count > 0)
        {
            float fIndex = normalisedTime * (linearPoints.Count - 1);
            int indexA = (int)fIndex;
            int indexB = Mathf.CeilToInt(fIndex);

            float d = fIndex - indexA;
            
            return Vector2.Lerp(linearPoints[indexA], linearPoints[indexB], d);
        }

        Debug.LogWarning("getposition has zero points!");
        return Vector2.zero;
    }
    public Vector2 StartPoint()
    {
        return controlPoints[0];
    }
    public Vector2 LastPoint()
    {
        return controlPoints[controlPoints.Count - 1];
    }
    public float Length()
    {
        float result = 0;
        for (int s=0; s < noOfSegments(); s++)
        {
            Vector2[] segmentPoints = GetSegmentPoints(s, Vector2.zero);
            float netLength = Vector2.Distance(segmentPoints[0], segmentPoints[1]) +
                              Vector2.Distance(segmentPoints[1], segmentPoints[2]) +
                              Vector2.Distance(segmentPoints[2], segmentPoints[3]);
            float curveLength = Vector2.Distance(segmentPoints[0], segmentPoints[3]) + (netLength / 2);
            result += curveLength;
        }
        return result;
    }
}
