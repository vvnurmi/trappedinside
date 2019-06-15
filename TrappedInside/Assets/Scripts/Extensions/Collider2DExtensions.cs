using System;
using UnityEngine;

public static class Collider2DExtensions
{
    /// <summary>
    /// Returns the collider's shape as a <see cref="Multipath2D"/>.
    /// Returns null if extraction failed.
    /// </summary>
    public static Multipath2D TryGetShapeAsPath(this Collider2D collider)
    {
        Vector2[][] shapePoints;

        switch (collider)
        {
            case BoxCollider2D box:
                Debug.Assert(box.edgeRadius == 0,
                    $"{nameof(TryGetShapeAsPath)} doesn't support rounded {nameof(BoxCollider2D)}");
                shapePoints = new[]
                {
                    new[]
                    {
                        // Wind counterclockwise.
                        box.offset + new Vector2(-box.size.x / 2, -box.size.y / 2),
                        box.offset + new Vector2(+box.size.x / 2, -box.size.y / 2),
                        box.offset + new Vector2(+box.size.x / 2, +box.size.y / 2),
                        box.offset + new Vector2(-box.size.x / 2, +box.size.y / 2),
                    }
                };
                break;

            case PolygonCollider2D poly:
                shapePoints = new Vector2[poly.pathCount][];
                for (int i = 0; i < poly.pathCount; i++)
                    shapePoints[i] = poly.GetPath(i);
                break;

            case CompositeCollider2D composite:
                Debug.Assert(composite.geometryType == CompositeCollider2D.GeometryType.Polygons,
                    $"{nameof(TryGetShapeAsPath)} only supports polygons in {nameof(CompositeCollider2D)}");
                shapePoints = new Vector2[composite.pathCount][];
                for (int i = 0; i < composite.pathCount; i++)
                {
                    shapePoints[i] = new Vector2[composite.GetPathPointCount(i)];
                    composite.GetPath(i, shapePoints[i]);
                }
                break;

            default:
                Debug.LogError($"Unsupported collider2D subclass {collider.GetType()} in {nameof(TryGetShapeAsPath)}");
                return null;
        }

        for (int i = 0; i < shapePoints.Length; i++)
            for (int j = 0; j < shapePoints[i].Length; j++)
                shapePoints[i][j] = collider.offset + (Vector2)collider.transform.TransformPoint(shapePoints[i][j]);
        return new Multipath2D(shapePoints);
    }
}
