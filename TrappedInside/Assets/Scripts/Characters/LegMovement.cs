using UnityEngine;

// Code adapted from Sebastian Lague's 2D Platformer Controller tutorial.
// https://github.com/SebLague/2DPlatformer-Tutorial

/// <summary>
/// Movement of a character that walks on legs.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(InputProvider))]
public class LegMovement : MonoBehaviour
{
    [Tooltip("Movement settings.")]
    public LegMovementParameters movement;

    [Tooltip("Ground collision settings.")]
    public RaycastCollider groundCollider;

    [Tooltip("The sound to play on jump.")]
    public AudioClip jumpSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterController2D characterController;
    private InputProvider inputProvider;
    private SpriteRenderer spriteRenderer;
    private float gravity;
    private float initialJumpSpeed;
    private float dampedJumpSpeed; // What jump speed becomes after Jump button is released.

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    // Modified during gameplay.
    private Vector2 velocity;
    private float velocityXSmoothing;

    private bool IsFacingRight => !spriteRenderer.flipX;

    // >>> MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController2D>();
        inputProvider = GetComponent<InputProvider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider.SetHitBox(boxCollider);

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);

        gravity = -(2 * movement.jumpHeightMax) / Mathf.Pow(movement.jumpApexTime, 2);
        initialJumpSpeed = Mathf.Abs(gravity) * movement.jumpApexTime;
        dampedJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * movement.jumpHeightMin);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        var oldVelocity = velocity;

        var input = inputProvider.GetInput();
        HandleVerticalInput(input);
        HandleHorizontalInput(input);

        var averageVelocity = Vector2.Lerp(oldVelocity, velocity, 0.5f);
        Move(averageVelocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (groundCollider.HasVerticalCollisions)
            velocity.y = 0;
        if (groundCollider.HasHorizontalCollisions)
            velocity.x = 0;

        animator.SetFloat("VerticalSpeed", velocity.y);
    }

    // <<< MonoBehaviour overrides

    private void HandleVerticalInput(PlayerInput input)
    {
        if (input.jumpPressed) Jump();
        if (input.jumpReleased) StopJumping();

        velocity.y += gravity * Time.deltaTime;
        animator.SetBool("Jumping", !groundCollider.collisions.below);
    }

    private void HandleHorizontalInput(PlayerInput input)
    {
        if (input.horizontal < 0 && IsFacingRight)
        {
            Flip();
        }
        else if (input.horizontal > 0 && !IsFacingRight)
        {
            Flip();
        }

        float targetVelocityX = characterController.state.CanMoveHorizontally
            ? input.horizontal * movement.maxSpeed
            : 0;
        float smoothTime = velocity.x * (targetVelocityX - velocity.x) < 0
            ? movement.maxSpeedStopTime
            : movement.maxSpeedReachTime;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        animator.SetFloat("Speed", Mathf.Abs(input.horizontal));
    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    private void Jump()
    {
        if (!groundCollider.collisions.below || !characterController.state.CanJump) return;

        velocity.y = initialJumpSpeed;
        timedAnimTriggers.Set("StartJump");
        audioSource.PlayOneShot(jumpSound);
    }

    private void StopJumping()
    {
        velocity.y = Mathf.Min(velocity.y, dampedJumpSpeed);
    }

    private void Move(Vector2 moveAmount)
    {
        groundCollider.UpdateRaycastOrigins();
        groundCollider.collisions.Reset();
        groundCollider.collisions.moveAmountOld = moveAmount;

        if (moveAmount.x != 0)
            groundCollider.collisions.faceDir = (int)Mathf.Sign(moveAmount.x);

        groundCollider.HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            groundCollider.VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }
}
