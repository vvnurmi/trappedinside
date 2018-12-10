using UnityEngine;
using System.Collections;

public class RaycastOrigins
{
    public Vector2 TopLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomLeft { get; set; }
    public Vector2 BottomRight { get; set; }
}

public class CollisionInfo
{
    public bool Above { get; set; }
    public bool Below { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }

    public Vector2 MoveAmountOld { get; set; }
    public int FaceDir { get; set; }

    public void Reset()
    {
        Above = false;
        Below = false;
        Left = false;
        Right = false;
    }
}

public class RaycastCollider
{

    private RaycastOrigins raycastOrigins = new RaycastOrigins();
    private Vector2Int rayCount = new Vector2Int();
    private Vector2 raySpacing;

    public float SkinWidth { get; set; }
    public BoxCollider2D BoxCollider { get; set; }
    public float ApproximateRaySpacing { get; set; }
    public CollisionInfo Collisions { get; private set; }
    public LayerMask GroundLayers { get; set; }

    public void Init()
    {
        CalculateRaySpacing();
        Collisions = new CollisionInfo
        {
            FaceDir = 1
        };
    }


    public void UpdateRaycastOrigins()
    {
        var bounds = BoxCollider.bounds;
        bounds.Expand(SkinWidth * -2);

        raycastOrigins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public void CalculateRaySpacing()
    {
        var bounds = BoxCollider.bounds;
        bounds.Expand(SkinWidth * -2);
        rayCount.x = Mathf.RoundToInt(bounds.size.y / ApproximateRaySpacing);
        rayCount.y = Mathf.RoundToInt(bounds.size.x / ApproximateRaySpacing);
        raySpacing.x = bounds.size.y / (rayCount.x - 1);
        raySpacing.y = bounds.size.x / (rayCount.y - 1);
    }

    public void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + SkinWidth;

        for (int i = 0; i < rayCount.y; i++)
        {
            var rayOrigin = (directionY == -1) ? raycastOrigins.BottomLeft : raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (raySpacing.y * i + moveAmount.x);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, GroundLayers);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
            if (!hit) continue;

            moveAmount.y = (hit.distance - SkinWidth) * directionY;
            rayLength = hit.distance;
            Collisions.Below = directionY == -1;
            Collisions.Above = directionY == 1;
        }
    }

    public void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = Collisions.FaceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + SkinWidth;

        if (Mathf.Abs(moveAmount.x) < SkinWidth)
            rayLength = 2 * SkinWidth;

        for (int i = 0; i < rayCount.x; i++)
        {
            var rayOrigin = (directionX == -1) ? raycastOrigins.BottomLeft : raycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (raySpacing.x * i);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, GroundLayers);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
            if (!hit || hit.distance == 0) continue;

            moveAmount.x = (hit.distance - SkinWidth) * directionX;
            rayLength = hit.distance;
            Collisions.Left = directionX == -1;
            Collisions.Right = directionX == 1;
        }
    }



}
