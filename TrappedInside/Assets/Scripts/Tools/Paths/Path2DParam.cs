using System.Diagnostics;

[DebuggerDisplay("{t}")]
public struct Path2DParam
{
    public float t;

    public static explicit operator Path2DParam(float x) => new Path2DParam { t = x };

    public int PointIndex => (int)t;
    public float Fraction => t - PointIndex;
}
