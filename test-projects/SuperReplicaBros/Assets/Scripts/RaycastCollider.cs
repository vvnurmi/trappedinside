using UnityEngine;
using System.Collections;

public class RaycastOrigins
{
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
}

public class CollisionInfo
{
    public bool above;
    public bool below;
    public bool left;
    public bool right;

    public Vector2 moveAmountOld;
    public int faceDir;

    public void Reset()
    {
        above = false;
        below = false;
        left = false;
        right = false;
    }
}

public class RaycastCollider
{

    private RaycastOrigins raycastOrigins = new RaycastOrigins();
    private Vector2Int rayCount = new Vector2Int();
    private Vector2 raySpacing;

    private readonly float skinWidth = 0.015f;
    private readonly float approximateRaySpacing = 0.25f;

    private BoxCollider2D boxCollider;
    private LayerMask hitLayers;

    public CollisionInfo collisions = new CollisionInfo();

    public RaycastCollider(BoxCollider2D collider, LayerMask layers)
    {
        boxCollider = collider;
        hitLayers = layers;
        CalculateRaySpacing();
    }

    public bool HasHorizontalCollisions {
        get { return collisions.left || collisions.right; }
    }

    public bool HasVerticalCollisions {
        get { return collisions.above || collisions.below; }
    }

    public void UpdateRaycastOrigins()
    {
        var bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        var bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        rayCount.x = Mathf.RoundToInt(bounds.size.y / approximateRaySpacing);
        rayCount.y = Mathf.RoundToInt(bounds.size.x / approximateRaySpacing);
        raySpacing.x = bounds.size.y / (rayCount.x - 1);
        raySpacing.y = bounds.size.x / (rayCount.y - 1);
    }

    public void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < rayCount.y; i++)
        {
            var rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (raySpacing.y * i + moveAmount.x);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, hitLayers);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
            if (!hit) continue;

            moveAmount.y = (hit.distance - skinWidth) * directionY;
            rayLength = hit.distance;
            collisions.below = directionY == -1;
            collisions.above = directionY == 1;
        }
    }

    public void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
            rayLength = 2 * skinWidth;

        for (int i = 0; i < rayCount.x; i++)
        {
            var rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (raySpacing.x * i);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, hitLayers);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
            if (!hit || hit.distance == 0) continue;

            moveAmount.x = (hit.distance - skinWidth) * directionX;
            rayLength = hit.distance;
            collisions.left = directionX == -1;
            collisions.right = directionX == 1;
        }
    }



}
