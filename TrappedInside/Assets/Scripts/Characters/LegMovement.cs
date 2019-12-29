using UnityEngine;

// Code adapted from Sebastian Lague's 2D Platformer Controller tutorial.
// https://github.com/SebLague/2DPlatformer-Tutorial

/// <summary>
/// Movement of a character that walks on legs.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(InputProvider))]
public class LegMovement : MonoBehaviour
{
    [Tooltip("Movement settings.")]
    public LegMovementParameters movement;

    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("The sound to play on jump.")]
    public AudioClip jumpSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterState characterState;
    private InputProvider inputProvider;
    private RaycastCollider groundCollider;
    private SpriteRenderer spriteRenderer;
    private float ladderCenterPosition;
    private float gravity;
    private float initialJumpSpeed;
    private float dampedJumpSpeed; // What jump speed becomes after Jump button is released.

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    // Modified during gameplay.
    private Vector2 velocity;
    private float velocityXSmoothing;

    private bool IsFacingRight => characterState.collisions.faceDir == 1;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterState = GetComponent<CharacterState>();
        inputProvider = GetComponent<InputProvider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider = new RaycastCollider(
            groundColliderConfig,
            boxCollider,
            characterState.collisions);

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
        CollisionInfo collisions = characterState.collisions;
        if (collisions.HasVerticalCollisions)
            velocity.y = 0;
        if (collisions.HasHorizontalCollisions)
            velocity.x = 0;

        var relativeSpeed = Mathf.InverseLerp(0, movement.maxSpeed, Mathf.Abs(velocity.y));
        animator.SetFloat("VerticalSpeed", relativeSpeed);
    }

    #endregion

    private void HandleVerticalInput(PlayerInput input)
    {
        if (characterState.isClimbing)
        {
            velocity.y = input.vertical * movement.maxSpeed / 2;
            velocity.x = 0;

            if (input.jumpPressed)
            {
                Jump(0.3f * initialJumpSpeed);
                characterState.isClimbing = false;
            }

            if (HasReachedLadderBottom(input))
                characterState.isClimbing = false;

        }
        else
        {
            if (input.jumpPressed)
                Jump(initialJumpSpeed);

            if (input.jumpReleased)
                StopJumping();

            velocity.y += gravity * Time.deltaTime;
            animator.SetBool("Jumping", !characterState.collisions.below);

            if (Mathf.Abs(input.vertical) > 0.8 && characterState.canClimb)
            {
                if (!characterState.collisions.below || input.vertical > 0)
                {
                    characterState.isClimbing = true;
                    //transform.position = new Vector2(ladderCenterPosition, transform.position.y);
                }
            }

        }
        animator.SetBool("Climbing", characterState.isClimbing);
    }

    private bool HasReachedLadderBottom(PlayerInput input) => input.vertical < 0 && characterState.collisions.below;

    private void HandleHorizontalInput(PlayerInput input)
    {
        if (characterState.isClimbing)
            return;

        var inputRequiresFlip =
            (input.horizontal < 0 && IsFacingRight) ||
            (input.horizontal > 0 && !IsFacingRight);
        if (inputRequiresFlip && characterState.CanChangeDirection)
            Flip();

        float targetVelocityX = characterState.CanMoveHorizontally
            ? input.horizontal * movement.maxSpeed
            : 0;
        float smoothTime = velocity.x * (targetVelocityX - velocity.x) < 0
            ? movement.maxSpeedStopTime
            : movement.maxSpeedReachTime;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);

        var relativeSpeed = Mathf.InverseLerp(0, movement.maxSpeed, Mathf.Abs(velocity.x));
        animator.SetFloat("Speed", relativeSpeed);
    }

    public void Flip()
    {
        var collisions = characterState.collisions;
        collisions.faceDir = -collisions.faceDir;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
    }

    private void Jump(float jumpSpeed)
    {
        if (characterState.CanJump)
        {
            velocity.y = jumpSpeed;
            timedAnimTriggers.Set("StartJump");
            audioSource.TryPlay(jumpSound);
        }
    }

    private void StopJumping()
    {
        velocity.y = Mathf.Min(velocity.y, dampedJumpSpeed);
    }

    private void Move(Vector2 moveAmount)
    {
        groundCollider.UpdateRaycastOrigins();

        CollisionInfo collisions = characterState.collisions;
        collisions.Reset();

        groundCollider.HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            groundCollider.VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsLadderLayer(collision))
        {
            ladderCenterPosition = collision.bounds.center.x;
            characterState.canClimb = true;
            Debug.Log("Ladder enter.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsLadderLayer(collision))
        {
            characterState.canClimb = false;
            Debug.Log("Ladder exit.");
        }
    }

    private bool IsLadderLayer(Collider2D collision) =>
        collision.gameObject.layer == LayerMask.NameToLayer("Ladder");
}
