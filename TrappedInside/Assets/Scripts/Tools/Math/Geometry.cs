using UnityEngine;

// Adapted from the 3D math function collection at
// https://wiki.unity3d.com/index.php/3d_Math_functions by Bit Barrel Media

public static class Geometry
{
    /// <summary>
    /// This function returns a point which is a projection from a point to a line.
    /// The line is regarded infinite. If the line is finite, use ProjectPointOnLineSegment() instead.
    /// </summary>
    public static Vector2 ProjectPointOnLine(Vector2 linePoint, Vector2 lineVec, Vector2 point)
    {
        //get vector from point on line to point in space
        Vector2 linePointToPoint = point - linePoint;

        float t = Vector2.Dot(linePointToPoint, lineVec);

        return linePoint + lineVec * t;
    }

    /// <summary>
    /// This function returns a point which is a projection from a point to a line segment.
    /// If the projected point lies outside of the line segment, the projected point will 
    /// be clamped to the appropriate line edge.
    /// If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
    /// </summary>
    public static Vector2 ProjectPointOnLineSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
    {
        Vector2 vector = linePoint2 - linePoint1;
        Vector2 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);
        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);

        switch (side)
        {
            case 0: return projectedPoint; //The projected point is on the line segment
            case 1: return linePoint1;
            case 2: return linePoint2;
            default: return Vector2.zero; //output is invalid
        }
    }

    /// <summary>
    /// This function finds out on which side of a line segment the point is located.
    /// The point is assumed to be on a line created by linePoint1 and linePoint2. If the point is not on
    /// the line segment, project it on the line using ProjectPointOnLine() first.
    /// Returns 0 if point is on the line segment.
    /// Returns 1 if point is outside of the line segment and located on the side of linePoint1.
    /// Returns 2 if point is outside of the line segment and located on the side of linePoint2.
    /// </summary>
    public static int PointOnWhichSideOfLineSegment(Vector2 linePoint1, Vector2 linePoint2, Vector2 point)
    {
        Vector2 lineVec = linePoint2 - linePoint1;
        Vector2 pointVec = point - linePoint1;

        float dot = Vector2.Dot(pointVec, lineVec);

        //point is on side of linePoint2, compared to linePoint1
        if (dot > 0)
        {
            //point is on the line segment
            if (pointVec.magnitude <= lineVec.magnitude)
                return 0;

            //point is not on the line segment and it is on the side of linePoint2
            else
                return 2;
        }

        //Point is not on side of linePoint2, compared to linePoint1.
        //Point is not on the line segment and it is on the side of linePoint1.
        else
            return 1;
    }
}
