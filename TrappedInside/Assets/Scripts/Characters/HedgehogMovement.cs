using System.Collections.Generic;
using UnityEngine;

public class HedgehogMovement : MonoBehaviour
{

    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    [Tooltip("Speed of movement along a surface, in world units per second.")]
    public float walkingSpeed = -0.1f;

    public float gravity = -5.0f;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private HitPoints hitPoints;
    private Animator animator;
    private Vector2 velocity;
    private DropIndicator dropIndicator;

    private void Start()
    {

        characterState = GetComponent<CharacterState>();
        var boxCollider = GetComponent<BoxCollider2D>();
        dropIndicator = GetComponentInChildren<DropIndicator>();
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
        if (velocity.x == 0)
        {
            velocity.x = walkingSpeed;
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
            velocity.x *= -1;
            transform.localScale = new Vector3(
                -transform.localScale.x,
                transform.localScale.y,
                transform.localScale.z);
        }
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
