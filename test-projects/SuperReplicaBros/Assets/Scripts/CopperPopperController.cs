using UnityEngine;

public class CopperPopperController : MonoBehaviour, ICollisionObject
{
    public Vector2 velocity = new Vector2(-2f, 0);
    public bool cocoon = false;
    public float gravity = -9.81f;

    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    private Animator animator;
    private RaycastCollider raycastCollider;

    public void OutOfBounds()
    {
        Destroy();
    }

    void Start()
    {
        raycastCollider = new RaycastCollider(GetComponent<BoxCollider2D>(), groundLayers);
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
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

    public void HandleCollision(CollisionDetails collisionDetails)
    {
        if (cocoon && velocity.x == 0)
        {
            float direction = Mathf.Sign(collisionDetails.velocity.x);
            velocity.x = direction * 10.0f;
        }
        else
        {
            if (collisionDetails.velocity.y < 0)
            {
                TakeDamage();
                collisionDetails.collisionObject.RecoilUp();
            }
            else if (collisionDetails.isAttack)
            {
                TakeDamage();
            }
            else
            {
                collisionDetails.collisionObject.TakeDamage();
            }
        }
    }

    public void TakeDamage()
    {
        animator.SetBool("Cocoon", true);
        velocity.x = 0;
        cocoon = true;
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity.x *= -1;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }

    public void RecoilUp()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Flip();
        }
    }

}
