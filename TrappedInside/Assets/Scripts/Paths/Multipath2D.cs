using UnityEngine;

/// <summary>
/// A set of closed paths in 2D space, one of which is current.
/// </summary>
public class Multipath2D
{
    private Vector2[][] pathPoints;
    private int pathIndex;

    public Path2D Current { get; set; }

    public Multipath2D(Vector2[][] pathPoints)
    {
        Debug.Assert(pathPoints != null && pathPoints.Length > 0);
        this.pathPoints = pathPoints;
        pathIndex = -1;
    }

    public void ChoosePath(int path)
    {
        Debug.Assert(path >= 0 && path < pathPoints.Length);
        pathIndex = path;
        Current = new Path2D { points = pathPoints[path] };
    }

    public Path2D GetPath(int path)
    {
        Debug.Assert(path >= 0 && path < pathPoints.Length);
        return new Path2D { points = pathPoints[path] };
    }

    public int FindClosestPath(Vector2 position)
    {
        int bestPath = 0;
        float bestDistance2 = float.MaxValue;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            var path = new Path2D { points = pathPoints[i] };
            var nearestPathParam = path.FindNearest(position);
            var nearestPathPoint = path.At(nearestPathParam);
            var distance2 = nearestPathPoint.Distance2(position);
            if (distance2 >= bestDistance2) continue;

            bestDistance2 = distance2;
            bestPath = i;
        }
        return bestPath;
    }

}
