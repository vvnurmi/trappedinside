using System.Linq;
using UnityEngine;

/// <summary>
/// Implements character movement by adhesively crawling on a surface, orienting along it.
/// </summary>
public class CrawlMovement : MonoBehaviour
{
    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float speed = 0.1f;

    private Vector2 movement;
    private Collider2D surface;
    private Path2D surfacePath;
    private Path2DParam surfaceParam;
    private readonly RaycastHit2D[] hits = new RaycastHit2D[4];

    public bool IsCrawling => surface != null;

    private void Update()
    {
        var oldMovement = movement;
        Vector2 deltaPosition;
        Vector2 worldDirection;

        // When crawling, follow the surface. Otherwise, fall by gravity.
        if (IsCrawling)
        {
            surfaceParam = surfacePath.Walk(surfaceParam, speed * Time.deltaTime);
            deltaPosition = surfacePath.At(surfaceParam) - (Vector2)transform.position;
            worldDirection = deltaPosition.normalized;
        }
        else
        {
            movement += Physics2D.gravity * Time.deltaTime;
            deltaPosition = Vector2.Lerp(movement, oldMovement, 0.5f) * Time.deltaTime;
            worldDirection = transform.TransformDirection(deltaPosition.normalized);
        }

        // See if we're going to collide into obstacles.
        var hitCount = Physics2D.RaycastNonAlloc(
            (Vector2)transform.position - worldDirection * groundColliderConfig.skinWidth,
            worldDirection,
            hits,
            deltaPosition.magnitude + groundColliderConfig.skinWidth,
            groundColliderConfig.hitLayers);

        // Find the nearest collision that isn't our current crawl surface.
        int hitIndex = -1;
        var hitSomethingNew = false;
        float shortestHitDistance = float.MaxValue;
        for (int i = 0; i < hitCount; i++)
        {
            if (hits[i].distance == 0) continue; // We're already inside the object, just go through.
            if (hits[i].collider == surface) continue; // We're crawling on this already.
            if (hits[i].distance >= shortestHitDistance) continue; // Not the nearest obstacle.

            shortestHitDistance = hits[i].distance;
            hitSomethingNew = true;
            hitIndex = i;
        }

        var debugColor = hitSomethingNew ? Color.red : Color.green;
        Debug.DrawLine(transform.position, (Vector2)transform.position + deltaPosition, debugColor, 1.0f);

        // Move, but not beyond a possible collision.
        if (hitSomethingNew)
            deltaPosition = deltaPosition.WithMagnitude(hits[hitIndex].distance);
        transform.Translate(deltaPosition, Space.World);

        // If we hit a new surface then make sure we are able to follow its shape.
        if (hitSomethingNew)
        {
            surfacePath = hits[hitIndex].collider.TryGetShapeAsPath();
            if (surfacePath == null)
            {
                hitSomethingNew = false;
                DetachFromSurface();
            }
        }

        // Attach to any crawlable surface in our way.
        if (hitSomethingNew)
        {
            surface = hits[hitIndex].collider;
            surfaceParam = surfacePath.FindNearest(hits[hitIndex].point);
            movement = Vector2.zero;
        }

        // Orient ourselves along the current crawl surface.
        if (IsCrawling)
        {
            var surfaceNormal = surfacePath.NormalAt(surfaceParam);
            var orientation = Quaternion.LookRotation(Vector3.forward, surfaceNormal);
            transform.SetPositionAndRotation(transform.position, orientation);
        }
    }

    public void DetachFromSurface()
    {
        surface = null;
        surfacePath = null;
        surfaceParam = (Path2DParam)0;
        movement = Vector2.zero;
        transform.SetPositionAndRotation(transform.position, Quaternion.LookRotation(Vector3.forward));
    }
}
