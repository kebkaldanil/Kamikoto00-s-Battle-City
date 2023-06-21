using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector2 Abs(this Vector2 vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
    public static float MaxAxis(this Vector2 vector)
    {
        return Mathf.Max(vector.x, vector.y);
    }
    public static float MinAxis(this Vector2 vector)
    {
        return Mathf.Min(vector.x, vector.y);
    }
    public static Vector2 LeftOnlyMaxAxis(this Vector2 vector)
    {
        if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y))
        {
            return new(vector.x, 0f);
        }
        return new(0f, vector.y);
    }
    public static Vector2Int Abs(this Vector2Int vector)
    {
        return new(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
    }
    public static int MaxAxis(this Vector2Int vector)
    {
        return Mathf.Max(vector.x, vector.y);
    }
    public static int MinAxis(this Vector2Int vector)
    {
        return Mathf.Min(vector.x, vector.y);
    }
    public static Vector2Int LeftOnlyMaxAxis(this Vector2Int vector)
    {
        if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y))
        {
            return new(vector.x, 0);
        }
        return new(0, vector.y);
    }
}
