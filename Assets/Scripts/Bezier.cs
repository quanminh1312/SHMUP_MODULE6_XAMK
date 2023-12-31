using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bezier
{
    public static Vector2 calculateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 p0 = Vector2.Lerp(a, b, t);
        Vector2 p1 = Vector2.Lerp(b, c, t);
        return Vector2.Lerp(p0, p1, t);
    }
    public static Vector2 calculateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
    {
        Vector2 p0 = calculateQuadratic(a, b, c, t);
        Vector2 p1 = calculateQuadratic(b, c, d, t);
        return Vector2.Lerp(p0, p1, t);
    }
}
