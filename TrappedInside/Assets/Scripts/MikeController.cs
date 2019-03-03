using System;
using UnityEngine;

// Code adapted from Sebastian Lague's 2D Platformer Controller tutorial.
// https://github.com/SebLague/2DPlatformer-Tutorial

public struct PlayerInput
{
    public bool fire1;
    public bool fire2;
    public bool jumpPressed;
    public bool jumpReleased;
    public float horizontal;
    public float vertical;
}

/// <summary>
/// Handles hero movement. Gravity is handcrafted.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class MikeController : MonoBehaviour, ICollisionObject
{
    [Tooltip("Maximum walking speed of the hero.")]
    public float maxSpeed = 5;

    [Tooltip("Seconds it takes to reach maximum speed from standstill.")]
    public float maxSpeedReachTime = 1;

    [Tooltip("Seconds it takes to stop to standstill from maximum speed.")]
    public float maxSpeedStopTime = 0.5f;

    [Tooltip("How high the hero jumps if Jump is touched only briefly.")]
    public float jumpHeightMin = 2;

    [Tooltip("How high the hero can jump when Jump is held down.")]
    public float jumpHeightMax = 4;

    [Tooltip("Seconds it takes for a jump to reach its apex.")]
    public float jumpApexTime = 1;

    [Tooltip("Minimum time between two consecutive hits.")]
    public float hitDelay = 1.0f;

    [Tooltip("Ground collision settings.")]
    public RaycastCollider groundCollider;

    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip punchSound;

    public int health = 5;

    // Set about once, probably in Start().
    private float gravity;
    private float initialJumpSpeed;
    private float dampedJumpSpeed; // What jump speed becomes after Jump button is released.
    private bool facingRight = true;
    private float nextHitAllowedAt = 0f;
    private Animator animator;
    private CircleCollider2D attackCollider;
    private AudioSource audioSource;

    // Modified during gameplay.
    private Vector2 velocity;
    private float velocityXSmoothing;
    private bool isControllable = true;
    private PlayerInput overrideControls; // Used unless 'isControllable'.
    private bool isInMelee;

    // Helpers
    private TimedAnimationTrigger startMeleeAnimTrigger;
    private TimedAnimationTrigger startJumpAnimTrigger;

    /// <summary>
    /// Invoked when the player dies.
    /// </summary>
    public event Action Death;

    private bool IsDead { get { return health <= 0; } }

    private void Start()
    {
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider.SetHitBox(boxCollider);

        gravity = -(2 * jumpHeightMax) / Mathf.Pow(jumpApexTime, 2);
        initialJumpSpeed = Mathf.Abs(gravity) * jumpApexTime;
        dampedJumpSpeed = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeightMin);
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;
        audioSource = GetComponent<AudioSource>();

        startMeleeAnimTrigger = new TimedAnimationTrigger(animator, "StartMelee", 0.1f);
        startJumpAnimTrigger = new TimedAnimationTrigger(animator, "StartJump", 0.1f);
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }

        startMeleeAnimTrigger.Update();
        startJumpAnimTrigger.Update();

        var oldVelocity = velocity;

        var input = GetInput();
        HandleFireInput(input);
        HandleVerticalInput(input);
        HandleHorizontalInput(input);

        animator.SetFloat("VerticalSpeed", velocity.y);

        var averageVelocity = Vector2.Lerp(oldVelocity, velocity, 0.5f);
        Move(averageVelocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (groundCollider.HasVerticalCollisions)
            velocity.y = 0;
        if (groundCollider.HasHorizontalCollisions)
            velocity.x = 0;
    }

    private PlayerInput GetInput()
    {
        return !isControllable
            ? overrideControls
            : new PlayerInput
            {
                fire1 = Input.GetButtonDown("Fire1"),
                fire2 = Input.GetButtonDown("Fire2"),
                jumpPressed = Input.GetButtonDown("Jump"),
                jumpReleased = Input.GetButtonUp("Jump"),
                horizontal = Input.GetAxis("Horizontal"),
                vertical = Input.GetAxis("Vertical"),
            };
    }

    private void HandleFireInput(PlayerInput input)
    {
        if (input.fire1)
        {
            startMeleeAnimTrigger.Set();
            PlaySound(punchSound);
        }
    }

    private void HandleVerticalInput(PlayerInput input)
    {
        if (input.jumpPressed) Jump();
        if (input.jumpReleased) StopJumping();

        velocity.y += gravity * Time.deltaTime;
        animator.SetBool("Jumping", !groundCollider.collisions.below);
    }

    private void HandleHorizontalInput(PlayerInput input)
    {
        if (input.horizontal < 0 && facingRight)
        {
            Flip();
        }
        else if (input.horizontal > 0 && !facingRight)
        {
            Flip();
        }

        float targetVelocityX = isInMelee
            ? 0
            : input.horizontal * maxSpeed;
        float smoothTime = velocity.x * (targetVelocityX - velocity.x) < 0
            ? maxSpeedStopTime
            : maxSpeedReachTime;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
        animator.SetFloat("Speed", Mathf.Abs(input.horizontal));
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void Jump()
    {
        if (!groundCollider.collisions.below || isInMelee) return;

        velocity.y = initialJumpSpeed;
        PlaySound(jumpSound);
        startJumpAnimTrigger.Set();
    }

    public void StopJumping()
    {
        velocity.y = Mathf.Min(velocity.y, dampedJumpSpeed);
    }

    public void Move(Vector2 moveAmount)
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

    public void TakeDamage()
    {
        if (IsDead || Time.time < nextHitAllowedAt)
            return;

        health--;

        if (IsDead)
            OnDeath();
        else
            OnDamage();
    }

    public void KillInstantly()
    {
        if (IsDead) return;
        health = 0;
        OnDeath();
    }

    private void OnDamage()
    {
        animator.Play("Damage");
        PlaySound(hitSound);
        velocity.x = 0;
        nextHitAllowedAt = Time.time + hitDelay;
    }

    private void OnDeath()
    {
        animator.Play("Death");
        PlaySound(hitSound);
        Death?.Invoke();
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }

    public void RecoilUp()
    {
        velocity.y = 10.0f;
    }


    public void AnimEvent_StartAttacking()
    {
        isInMelee = true;
        attackCollider.enabled = true;
    }

    public void AnimEvent_StopAttacking()
    {
        isInMelee = false;
        attackCollider.enabled = false;
        animator.SetTrigger("StopMelee");
    }

    /// <summary>
    /// Makes the player use the given input.
    /// </summary>
    public void OverridePlayerControl(PlayerInput input)
    {
        isControllable = false;
        overrideControls = input;
    }

    /// <summary>
    /// Resumes player control over the character.
    /// </summary>
    public void ResumePlayerControl()
    {
        isControllable = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.SendMessage(
                "HandleCollision",
                new CollisionDetails { velocity = velocity, collisionObject = this, isAttack = attackCollider.IsTouching(collision) }
                );
        }
    }
}
