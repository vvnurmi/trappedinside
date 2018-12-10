using System;
using UnityEngine;

// Code adapted from Sebastian Lague's 2D Platformer Controller tutorial.
// https://github.com/SebLague/2DPlatformer-Tutorial


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

    [Tooltip("How high the hero jumps if Jump is touched only briefly.")]
    public float jumpHeightMin = 2;

    [Tooltip("How high the hero can jump when Jump is held down.")]
    public float jumpHeightMax = 4;

    [Tooltip("Seconds it takes for a jump to reach its apex.")]
    public float jumpApexTime = 1;

    [Tooltip("Leeway around the box collider.")]
    public const float skinWidth = .015f;

    [Tooltip("How far apart to shoot ground collision rays. The actual distance may differ slightly.")]
    public const float approximateRaySpacing = 0.25f;

    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    // Set about once, probably in Start().
    private float gravity;
    private float initialJumpSpeed;
    private float dampedJumpSpeed; // What jump speed becomes after Jump button is released.
    private bool facingRight = true;
    private Animator animator;
    private RaycastCollider raycastCollider;

    // Modified during gameplay.
    private Vector2 velocity;
    private float velocityXSmoothing;

    /// <summary>
    /// Invoked when the player dies.
    /// </summary>
    public event Action Death;

    public void OutOfBounds()
    {
        Death.Invoke();
    }

    private void Start()
    {
        raycastCollider = new RaycastCollider
        {
            SkinWidth = skinWidth,
            BoxCollider = GetComponent<BoxCollider2D>(),
            ApproximateRaySpacing = approximateRaySpacing,
            GroundLayers = groundLayers
        };
        raycastCollider.Init();
        gravity = -(2 * jumpHeightMax) / Mathf.Pow(jumpApexTime, 2);
        initialJumpSpeed = Mathf.Abs(gravity) * jumpApexTime;
        dampedJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeightMin);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        var oldVelocity = velocity;

        HandleVerticalInput();
        HandleHorizontalInput();

        var averageVelocity = Vector2.Lerp(oldVelocity, velocity, 0.5f);
        Move(averageVelocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (raycastCollider.Collisions.Above || raycastCollider.Collisions.Below)
            velocity.y = 0;
        if (raycastCollider.Collisions.Left || raycastCollider.Collisions.Right)
            velocity.x = 0;
    }

    private void HandleVerticalInput() {
        var isJumping = Input.GetButtonDown("Jump");
        var stopJumping = Input.GetButtonUp("Jump");

        if (isJumping) Jump();
        if (stopJumping) StopJumping();

        velocity.y += gravity * Time.deltaTime;
        animator.SetBool("Jumping", !raycastCollider.Collisions.Below);
    }

    private void HandleHorizontalInput() {
        var horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput < 0 && facingRight) {
            Flip();
        } else if (horizontalInput > 0 && !facingRight) {
            Flip();
        }

        float targetVelocityX = horizontalInput * maxSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, maxSpeedReachTime);
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Jump()
    {
        if (raycastCollider.Collisions.Below)
            velocity.y = initialJumpSpeed;
    }

    public void StopJumping()
    {
        velocity.y = Mathf.Min(velocity.y, dampedJumpSpeed);
    }

    public void Move(Vector2 moveAmount)
    {
        raycastCollider.UpdateRaycastOrigins();
        raycastCollider.Collisions.Reset();
        raycastCollider.Collisions.MoveAmountOld = moveAmount;

        if (moveAmount.x != 0)
            raycastCollider.Collisions.FaceDir = (int)Mathf.Sign(moveAmount.x);

        raycastCollider.HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            raycastCollider.VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }
}
