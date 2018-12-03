﻿using UnityEngine;

// Code adapted from Sebastian Lague's 2D Platformer Controller tutorial.
// https://github.com/SebLague/2DPlatformer-Tutorial

public struct RaycastOrigins
{
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
}

public struct CollisionInfo
{
    public bool above, below;
    public bool left, right;

    public Vector2 moveAmountOld;
    public int faceDir;

    public void Reset()
    {
        above = below = false;
        left = right = false;
    }
}

/// <summary>
/// Handles hero movement. Gravity is handcrafted.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Maximum walking speed of the hero.")]
    public float maxSpeed = 5;

    [Tooltip("Seconds it takes to reach maximum speed from standstill.")]
    public float maxSpeedReachTime = 1;

    [Tooltip("How high the hero can jump.")]
    public float jumpHeight = 3;

    [Tooltip("Seconds it takes for a jump to reach its apex.")]
    public float jumpApexTime = 1;

    [Tooltip("Leeway around the box collider.")]
    public const float skinWidth = .015f;

    [Tooltip("How far apart to shoot ground collision rays. The actual distance may differ slightly.")]
    public float approximateRaySpacing = 0.25f;

    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    // Set about once, probably in Start().
    private float gravity;
    private float initialJumpSpeed;
    [HideInInspector]
    private BoxCollider2D boxCollider;
    [HideInInspector]
    public Vector2Int rayCount;
    [HideInInspector]
    public Vector2 raySpacing;
    [HideInInspector]
    public RaycastOrigins raycastOrigins;

    // Modified during gameplay.
    private Vector2 velocity;
    private float velocityXSmoothing;
    public CollisionInfo collisions;

    private void Start()
    {
        gravity = -(2 * jumpHeight) / Mathf.Pow(jumpApexTime, 2);
        initialJumpSpeed = Mathf.Abs(gravity) * jumpApexTime;
        boxCollider = GetComponent<BoxCollider2D>();
        collisions.faceDir = 1;
        CalculateRaySpacing();
    }

    private void Update()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        var isJumping = Input.GetButtonDown("Jump");

        if (isJumping) Jump();

        float targetVelocityX = horizontalInput * maxSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, maxSpeedReachTime);
        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (collisions.above || collisions.below)
            velocity.y = 0;
        if (collisions.left || collisions.right)
            velocity.x = 0;
    }

    public void Jump()
    {
        if (collisions.below)
            velocity.y = initialJumpSpeed;
    }

    public void Move(Vector2 moveAmount)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveAmountOld = moveAmount;

        if (moveAmount.x != 0)
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
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

    private void HorizontalCollisions(ref Vector2 moveAmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth)
            rayLength = 2 * skinWidth;

        for (int i = 0; i < rayCount.x; i++)
        {
            var rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (raySpacing.x * i);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, groundLayers);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);
            if (!hit || hit.distance == 0) continue;

            moveAmount.x = (hit.distance - skinWidth) * directionX;
            rayLength = hit.distance;
            collisions.left = directionX == -1;
            collisions.right = directionX == 1;
        }
    }

    private void VerticalCollisions(ref Vector2 moveAmount)
    {
        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < rayCount.y; i++)
        {
            var rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (raySpacing.y * i + moveAmount.x);
            var hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, groundLayers);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);
            if (!hit) continue;

            moveAmount.y = (hit.distance - skinWidth) * directionY;
            rayLength = hit.distance;
            collisions.below = directionY == -1;
            collisions.above = directionY == 1;
        }
    }
}