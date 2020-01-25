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

    public float runningSpeed = 2.0f;
    public float gravity = -5.0f;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private Vector2 velocity;
    private AttackTrigger attackTrigger;
    private AlertTrigger alertTrigger;
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
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var oldYSpeed = velocity.y;

        if (!attackStarted)
        {
            if (attackTrigger.PlayerInAttackRange)
            {
                SetAnimatorState(running);
                attackStarted = true;
                if (attackTrigger.PlayerInLeft)
                    velocity = new Vector2(-runningSpeed, 0);
                else
                    velocity = new Vector2(runningSpeed, 0);
            }
            else if (alertTrigger.PlayerInVisionRange)
            {
                SetAnimatorState(alerted);
                velocity = new Vector2(0, 0);
            }
            else
            {
                ClearAnimatorState();
                velocity = new Vector2(0, 0);
            }
        }
        velocity.y = oldYSpeed + gravity * Time.deltaTime;

        Move(velocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        var collisions = characterState.collisions;
        if (collisions.HasVerticalCollisions)
            velocity.y = 0;
        if (collisions.HasHorizontalCollisions)
            velocity.x = 0;
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
