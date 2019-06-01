using UnityEngine;

public static class Collider2DExtensions
{
    /// <summary>
    /// Returns the collider's shape as a <see cref="Path2D"/>.
    /// Returns null if extraction failed.
    /// </summary>
    public static Path2D TryGetShapeAsPath(this Collider2D collider)
    {
        Vector2[] shapePoints;

        switch (collider)
        {
            case BoxCollider2D box:
                Debug.Assert(box.edgeRadius == 0, $"{nameof(TryGetShapeAsPath)} doesn't support rounded {nameof(BoxCollider2D)}");
                shapePoints = new[]
                {
                    // Wind counterclockwise.
                    box.offset + new Vector2(-box.size.x / 2, -box.size.y / 2),
                    box.offset + new Vector2(+box.size.x / 2, -box.size.y / 2),
                    box.offset + new Vector2(+box.size.x / 2, +box.size.y / 2),
                    box.offset + new Vector2(-box.size.x / 2, +box.size.y / 2),
                };
                break;

            case PolygonCollider2D poly:
                Debug.Assert(poly.pathCount == 1, $"{nameof(TryGetShapeAsPath)} doesn't support holes in {nameof(PolygonCollider2D)}");
                shapePoints = poly.GetPath(0);
                break;

            default:
                Debug.LogError($"Unsupported collider2D subclass {collider.GetType()} in {nameof(TryGetShapeAsPath)}");
                return null;
        }

        for (int i = 0; i < shapePoints.Length; i++)
            shapePoints[i] = collider.transform.TransformPoint(shapePoints[i]);
        return new Path2D { points = shapePoints };
    }
}
