using UnityEngine;

/// <summary>
/// Implements character movement by adhesively crawling on a surface, orienting along it.
/// </summary>
public class CrawlMovement : MonoBehaviour
{
    [Tooltip("Which collision layers are to be hit.")]
    public LayerMask hitLayers;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float speed;

    private Vector2 movement;
    private Collider2D surface;
    private Path2D surfacePath;
    private Path2DParam surfaceParam;

    public bool IsFalling => surface == null;

    private void Update()
    {
        var oldMovement = movement;
        Vector2 deltaPosition;
        Vector2 worldDirection;
        if (IsFalling)
        {
            movement += Physics2D.gravity * Time.deltaTime;
            deltaPosition = Vector2.Lerp(movement, oldMovement, 0.5f) * Time.deltaTime;
            worldDirection = transform.TransformDirection(deltaPosition.normalized);
        }
        else
        {
            surfaceParam = surfacePath.Walk(surfaceParam, speed * Time.deltaTime);
            deltaPosition = surfacePath.At(surfaceParam) - (Vector2)transform.position;
            worldDirection = deltaPosition.normalized;
        }
        var distance = deltaPosition.magnitude;
        var hit = Physics2D.Raycast(transform.position, worldDirection, distance, hitLayers);
        var hitSomethingNew = hit && hit.collider != surface;

        var debugColor = hitSomethingNew ? Color.red : Color.green;
        Debug.DrawLine(transform.position, (Vector2)transform.position + deltaPosition, debugColor, 1.0f);

        // Move, but not beyond a possible collision.
        if (hitSomethingNew)
            deltaPosition = deltaPosition.WithMagnitude(hit.distance);
        transform.Translate(deltaPosition, Space.World);

        if (!hitSomethingNew)
        {
            if (!IsFalling)
            {
                // Orient along the surface we're crawling on.
                var surfaceNormal = surfacePath.NormalAt(surfaceParam);
                var orientation = Quaternion.LookRotation(Vector3.forward, surfaceNormal);
                transform.SetPositionAndRotation(transform.position, orientation);
            }
        }
        else if ((surfacePath = hit.collider.TryGetShapeAsPath()) != null)
        {
            // Attach to the new collider and crawl around its surface.
            surface = hit.collider;
            surfaceParam = surfacePath.FindNearest(hit.point);
            movement = Vector2.zero;
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
