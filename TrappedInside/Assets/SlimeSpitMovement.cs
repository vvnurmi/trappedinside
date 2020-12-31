using System.Collections.Generic;
using UnityEngine;

public class SlimeSpitMovement : MonoBehaviour
{
    [Tooltip("Ground collision settings.")]
    public RaycastColliderConfig groundColliderConfig;

    public float gravity = -5.0f;

    private CharacterState characterState;
    private RaycastCollider groundCollider;
    private HitPoints hitPoints;
    private Vector2 velocity;

    private void Start()
    {
        characterState = GetComponent<CharacterState>();
        var boxCollider = GetComponent<BoxCollider2D>();
        groundCollider = new RaycastCollider(
                groundColliderConfig,
                boxCollider,
                characterState.collisions);
        hitPoints = GetComponent<HitPoints>();
        velocity = InitialVelocity;
    }

    void FixedUpdate()
    {
        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);

        var collisions = characterState.collisions;
        if (collisions.HasVerticalCollisions || collisions.HasHorizontalCollisions)
        {
            velocity = Vector2.zero;
            Destroy(gameObject);
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

    public Vector2 InitialVelocity { get; set; }

}
