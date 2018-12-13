using UnityEngine;

public class CopperPopperController : MonoBehaviour
{
    public Vector2 velocity = new Vector2(-2f, 0);
    public bool cocoon = false;
    public float gravity = -9.81f;
    public float cocoonRecoveryTime = 10.0f;

    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    private Animator animator;
    private float cocoonTime;
    private RaycastCollider raycastCollider;

    public void OutOfBounds()
    {
        Destroy();
    }

    void Start()
    {
        cocoonTime = -cocoonRecoveryTime;
        raycastCollider = new RaycastCollider(GetComponent<BoxCollider2D>(), groundLayers);
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(Time.time > cocoonTime + cocoonRecoveryTime && cocoon)
        {
            cocoon = false;
            animator.SetBool("Cocoon", false);
            velocity.x = -2;
        }

        if (cocoon)
        {
            return;
        }

        velocity.y += gravity * Time.deltaTime;
        Move(velocity * Time.deltaTime);

        // Stop movement in directions where we have collided.
        if (raycastCollider.HasVerticalCollisions)
            velocity.y = 0;
        if (raycastCollider.HasHorizontalCollisions)
            Flip();
    }

    public void Move(Vector2 moveAmount)
    {
        raycastCollider.UpdateRaycastOrigins();
        raycastCollider.collisions.Reset();
        raycastCollider.collisions.moveAmountOld = moveAmount;

        if (moveAmount.x != 0)
            raycastCollider.collisions.faceDir = (int)Mathf.Sign(moveAmount.x);

        raycastCollider.HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0)
            raycastCollider.VerticalCollisions(ref moveAmount);

        transform.Translate(moveAmount);
    }

    public void TakeDamage()
    {
        animator.SetBool("Cocoon", true);
        velocity.x = 0;
        cocoonTime = Time.time;
        cocoon = true;
    }

    void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity.x *= -1;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
}
