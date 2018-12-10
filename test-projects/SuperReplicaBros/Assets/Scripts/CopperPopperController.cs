using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopperPopperController : MonoBehaviour {
    public Vector2 velocity = new Vector2(-2f, 0);
    public Transform wallCheck;
    public bool cocoon = false;
    public float gravity = -9.81f;

    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    private Animator animator;
    private bool cocoonHandled = false;

    private RaycastCollider raycastCollider;

    public void OutOfBounds() {
        Destroy();
    }

    // Use this for initialization
    void Start() {
        raycastCollider = new RaycastCollider
        {
            SkinWidth = PlayerController.skinWidth,
            BoxCollider = GetComponent<BoxCollider2D>(),
            ApproximateRaySpacing = PlayerController.approximateRaySpacing,
            GroundLayers = groundLayers
        };
        raycastCollider.Init();

        animator = GetComponent<Animator>();
    }

    void FixedUpdate() {

        if (cocoon && cocoonHandled) {
            return;
        }

        if (cocoon && !cocoonHandled) {
            HandleCocoon();
            return;
        }
        velocity.y += gravity * Time.deltaTime;

        Move(velocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (raycastCollider.Collisions.Above || raycastCollider.Collisions.Below)
            velocity.y = 0;
        if (raycastCollider.Collisions.Left || raycastCollider.Collisions.Right)
            Flip();

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


    void HandleCocoon() {
        animator.SetBool("Cocoon", true);
        velocity.x = 0;
        cocoonHandled = true;
    }

    void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity.x *= -1;
    }

    void Destroy() {
        Destroy(gameObject);
    }
}
