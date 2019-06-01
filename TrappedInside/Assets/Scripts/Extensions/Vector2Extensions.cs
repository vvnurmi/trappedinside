using UnityEngine;

public static class Vector2Extensions
{
    public static float Distance2(this Vector2 a, Vector2 b) =>
        Vector2.SqrMagnitude(b - a);

    public static Vector2 WithMagnitude(this Vector2 vector, float magnitude) =>
        vector.normalized * magnitude;
}
