using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Functions needed for collision info
public static class CollisionLib
{
    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
    }

    public static float Cross(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.y - v1.y * v2.x;
    }

    public static Vector2 Project(Vector2 v1, Vector2 v2)
    {
        float k = Vector2.Dot(v1, v2) / Vector2.Dot(v2, v2);
        return new Vector2(k * v2.x, k * v2.y);
    }

}

//Shape Primatives
public class CirclePrimitive
{
    public Vector2 center;
    public float radius;

    public CirclePrimitive(Vector2 center, float radius)
    {
        this.center = center;
        this.radius = radius;
    }


}
public class LinePrimitive
{
    public Vector2 start;
    public Vector2 end;

    public Vector2 vectorLine;

    public LinePrimitive(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;

        vectorLine = end - start;
    }

    public void UpdateLine(Vector2 start, Vector2 end)
    {
        //Update the line with new info
        this.start = start;
        this.end = end;

        vectorLine = end - start;
    }

}

