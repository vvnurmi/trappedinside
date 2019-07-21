using UnityEngine;

/// <summary>
/// A closed path in 2D space.
/// </summary>
public class Path2D
{
    private const double DegenerateSegmentLength = 1e-6;

    public Vector2[] points;

    public Path2DParam NextPoint(Path2DParam p) =>
        (Path2DParam)((Mathf.Floor(p.t) + 1) % points.Length);

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
    /// Returns the normal of the path at <paramref name="p"/>.
    /// Winding direction is assumed to be counterclockwise, so the
    /// normal points to the right relative to the path walk direction.
    /// </summary>
    public Vector2 NormalAt(Path2DParam p)
    {
        int pointIndex = p.PointIndex;
        Debug.Assert(pointIndex >= 0 && pointIndex < points.Length);
        var segmentStart = points[pointIndex];
        var segmentEnd = points[(pointIndex + 1) % points.Length];
        return Vector2.Perpendicular((segmentStart - segmentEnd).normalized);
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
        var degeneratePathTimeout = points.Length + 1;
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

            // Break out of walking a degenerate path.
            degeneratePathTimeout = remainingSegmentDistance > 1e-3
                ? points.Length
                : degeneratePathTimeout - 1;
            if (degeneratePathTimeout <= 0)
                return start;
        }
    }

    /// <summary>
    /// Returns the parameter on the path that is closest to <paramref name="position"/>.
    /// </summary>
    public Path2DParam FindNearest(Vector2 position)
    {
        // Brute force solution: loop over segments to find which is closest to the position.
        var bestParam = (Path2DParam)0;
        var bestDistance2 = float.MaxValue;
        var nextP = (Path2DParam)1;

        for (var p = (Path2DParam)0; nextP.t > 0; p = nextP)
        {
            nextP = NextPoint(p);
            var segmentStart = At(p);
            var segmentEnd = At(nextP);
            var segmentLength = Vector2.Distance(segmentEnd, segmentStart);
            if (segmentLength < DegenerateSegmentLength) continue;

            var projection = Geometry.ProjectPointOnLineSegment(segmentStart, segmentEnd, position);
            var distance2 = position.Distance2(projection);
            if (distance2 < bestDistance2)
            {
                bestDistance2 = distance2;
                var projectionFromP = Vector2.Distance(projection, segmentStart);
                bestParam = Add(p, projectionFromP / segmentLength);
            }
        }

        return (Path2DParam)(bestParam.t % points.Length);
    }
}
