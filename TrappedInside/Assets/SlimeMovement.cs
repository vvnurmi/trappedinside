using System;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{

    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float walkingSpeed = -0.1f;

    public float gravity = -5.0f;

    public GameObject slimeSpitObject;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private HitPoints hitPoints;
    private Animator animator;
    private Vector2 velocity;
    private DropIndicator dropIndicator;
    private AttackTrigger attackTrigger;
    private float previousSpitTime = 0.0f;
    private readonly float spitInterval = 2.0f;
    private readonly List<string> animationStates = new List<string>{ "Idle", "Move", "Spit"};

    private void Start()
    {
        characterState = GetComponent<CharacterState>();
        var boxCollider = GetComponent<BoxCollider2D>();
        dropIndicator = GetComponentInChildren<DropIndicator>();
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        groundCollider = new RaycastCollider(
                groundColliderConfig,
                boxCollider,
                characterState.collisions);
        hitPoints = GetComponent<HitPoints>();
        animator = GetComponent<Animator>();
        velocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (attackTrigger.PlayerInAttackRange)
        {
            velocity.x = 0;
            if (Time.time - previousSpitTime > spitInterval)
            {
                previousSpitTime = Time.time;
                SetAnimationState("Spit");
            }
            else
            {
                SetAnimationState("Idle");
            }
        }
        else
        {
            SetAnimationState("Idle");
        }

        //Slime can move only after it has finished spitting animation.
        var inSpittingState = animator.GetCurrentAnimatorStateInfo(0).IsName("Slime spit");
        if (!attackTrigger.PlayerInAttackRange && !inSpittingState)
        {
            velocity.x = walkingSpeed;
            SetAnimationState("Move");
        }

        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);

        var collisions = characterState.collisions;
        if (collisions.HasVerticalCollisions)
        {
            velocity.y = 0;
        }

        if (collisions.HasHorizontalCollisions || !dropIndicator.IsGroundAhead)
        {
            walkingSpeed *= -1;
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z);
        }
    }

    private void SetAnimationState(string state)
    {
        animationStates.ForEach(animationState => animator.SetBool(animationState, false));
        animator.SetBool(state, true);
    }

    private void SpitAnimationEnded()
    {
        Spit();
    }

    private void Spit()
    {
        var slimeSpit = Instantiate(slimeSpitObject, transform.position + new Vector3(0f, -0.01f, 0), Quaternion.identity);
        var slimeSpitMovement = slimeSpit.GetComponent<SlimeSpitMovement>();

        var yStartVelocity = 2.0f;
        var timeInAir = (yStartVelocity + Mathf.Sqrt(yStartVelocity * yStartVelocity - 2 * gravity * 0.08f)) / -gravity;
        var xDistance = attackTrigger.PlayerPosition?.x - transform.position.x;

        var xVelocity = xDistance / timeInAir;

        if (xVelocity.HasValue)
        {
            slimeSpitMovement.InitialVelocity = 
                new Vector2(
                    GetLimitedInitialVelocity(attackTrigger.PlayerInLeft, 0.3f, xVelocity.Value), 
                    yStartVelocity);
        }
    }

    private float GetLimitedInitialVelocity(bool playerInLeft, float minVelocity, float velocity)
    {
        if (attackTrigger.PlayerInLeft)
            return Mathf.Min(-minVelocity, velocity);
        else
            return Mathf.Max(minVelocity, velocity);
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

}
