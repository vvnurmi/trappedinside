using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerMovement : MonoBehaviour
{

    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float walkingSpeed = 0.1f;

    private static readonly string alerted = "Alerted";
    private static readonly string running = "Running";

    private readonly List<string> animatorStates = new List<string> { alerted, running };

    public float runningSpeed = 1.5f;
    public float gravity = -5.0f;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private Vector2 velocity;
    private AttackTrigger attackTrigger;
    private AlertTrigger alertTrigger;
    private HitPoints hitPoints;
    private Animator animator;
    private bool attackStarted = false;

    private void Start()
    {

        characterState = GetComponent<CharacterState>();
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider = new RaycastCollider(
                groundColliderConfig,
                boxCollider,
                characterState.collisions);
        velocity = Vector2.zero;
        attackTrigger = GetComponentInChildren<AttackTrigger>();
        alertTrigger = GetComponentInChildren<AlertTrigger>();
        hitPoints = GetComponent<HitPoints>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var oldVelocity = velocity;
        float xSpeed = oldVelocity.x;

        if (!attackStarted)
        {
            if (attackTrigger.PlayerInAttackRange)
            {
                SetAnimatorState(running);
                attackStarted = true;
                xSpeed = attackTrigger.PlayerInLeft ? -runningSpeed : runningSpeed;
            }
            else if (alertTrigger.PlayerInVisionRange)
            {
                SetAnimatorState(alerted);
            }
            else
            {
                ClearAnimatorState();
            }
        }

        velocity = new Vector2(xSpeed, oldVelocity.y + gravity * Time.deltaTime);
        var averageVelocity = Vector2.Lerp(oldVelocity, velocity, 0.5f);
        Move(averageVelocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        var collisions = characterState.collisions;
        if (collisions.HasVerticalCollisions)
            velocity.y = 0;
        if (collisions.HasHorizontalCollisions)
        {
            hitPoints.Damage(1);
            velocity.x = 0;
        }
    }


    private void SetAnimatorState(string state)
    {
        ClearAnimatorState();
        animator.SetBool(state, true);
    }

    private void ClearAnimatorState()
    {
        foreach (var animatorState in animatorStates)
            animator.SetBool(animatorState, false);
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
