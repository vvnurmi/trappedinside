using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalumMovement : MonoBehaviour
{
    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float walkingSpeed = 0.5f;

    private static readonly string idle = "Idle";
    private static readonly string walking = "Walk";

    private readonly List<string> animatorStates = new List<string> { idle, walking };
    public float gravity = -5.0f;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private Vector2 velocity;
    private AttackTrigger frontAttackTrigger;
    private AttackTrigger backAttackTrigger;
    private HitPoints hitPoints;
    private Animator animator;
    private SpriteRenderer spriteRendered;
    private Transform pole;
    private readonly float maxDistanceFromPole = 0.4f;
    private bool facingLeft = true;

    private void Start()
    {

        characterState = GetComponent<CharacterState>();
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider = new RaycastCollider(
                groundColliderConfig,
                boxCollider,
                characterState.collisions);
        velocity = Vector2.zero;
        frontAttackTrigger = transform.GetChild(0).GetComponent<AttackTrigger>();
        backAttackTrigger = transform.GetChild(1).GetComponent<AttackTrigger>();
        hitPoints = GetComponent<HitPoints>();
        animator = GetComponent<Animator>();
        spriteRendered = GetComponent<SpriteRenderer>();
        pole = transform.parent.GetChild(0);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var oldVelocity = velocity;
        float xSpeed = oldVelocity.x;

        if (frontAttackTrigger.PlayerInAttackRange && backAttackTrigger.PlayerInAttackRange)
        {
            xSpeed = 0;
        }
        else if (frontAttackTrigger.PlayerInAttackRange)
        {
            SetAnimatorState(walking);
            xSpeed = GetSpeed();
        }
        else if (backAttackTrigger.PlayerInAttackRange)
        {
            SetAnimatorState(walking);
            Flip();
            xSpeed = GetSpeed();
        }

        if (facingLeft && TooFarLeft || !facingLeft && TooFarRight)
        {
            xSpeed = 0;
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
            velocity.x = 0;
        }
    }

    private bool TooFarLeft => pole.position.x - transform.position.x > maxDistanceFromPole;
    private bool TooFarRight => transform.position.x - pole.position.x > maxDistanceFromPole;

    private float GetSpeed()
    {
        if (facingLeft)
            return -walkingSpeed;
        else
            return walkingSpeed;

    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        transform.localScale = new Vector3(
            -transform.localScale.x,
            transform.localScale.y,
            transform.localScale.z);
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
