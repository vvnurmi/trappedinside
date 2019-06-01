using UnityEngine;

/// <summary>
/// A closed path in 2D space.
/// </summary>
public class Path2D
{
    public Vector2[] points;

    public Path2DParam NextPoint(Path2DParam p) =>
        (Path2DParam)(Mathf.Floor(p.t + 1) % points.Length);

    public Path2DParam Add(Path2DParam p, float u) =>
        (Path2DParam)((p.t + u) % points.Length);

    public Vector2 At(Path2DParam p)
    {
        int pointIndex = p.PointIndex;
        Debug.Assert(pointIndex >= 0 && pointIndex < points.Length);
        var segmentStart = points[pointIndex];
        var segmentEnd = points[(pointIndex + 1) % points.Length];
        return Vector2.Lerp(segmentStart, segmentEnd, p.Fraction);
    }

    /// <summary>
    /// Returns the parameter on the path you get to by walking
    /// <paramref name="worldDistance"/> from <paramref name="start"/>
    /// along the path.
    /// </summary>
    public Path2DParam Walk(Path2DParam start, float worldDistance)
    {
        Debug.Assert(start.PointIndex >= 0 && start.PointIndex < points.Length);
        var p = start;
        var remainingDistance = worldDistance;
        while (true)
        {
            int pointIndex = p.PointIndex;
            var segmentStart = points[pointIndex];
            var segmentEnd = points[(pointIndex + 1) % points.Length];
            var position = Vector2.Lerp(segmentStart, segmentEnd, p.Fraction);
            var remainingSegmentDistance = Vector2.Distance(position, segmentEnd);
            if (remainingDistance < remainingSegmentDistance)
            {
                var segmentWalkParam = remainingDistance / Vector2.Distance(segmentStart, segmentEnd);
                return Add(p, segmentWalkParam);
            }

            p = NextPoint(p);
            remainingDistance -= remainingSegmentDistance;
        }
    }
}
