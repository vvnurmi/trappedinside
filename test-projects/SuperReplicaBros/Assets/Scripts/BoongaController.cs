using UnityEngine;

public class BoongaController : MonoBehaviour, ICollisionObject
{

    public Vector2 velocity = new Vector2(-2f, 0);
    public float gravity = -9.81f;
    public bool dead = false;
    [Tooltip("Which collision layers are considered ground.")]
    public LayerMask groundLayers;

    private Animator animator;
    private RaycastCollider raycastCollider;

    public void OutOfBounds()
    {
        Destroy();
    }

	// Use this for initialization
	void Start ()
    {
        raycastCollider = new RaycastCollider(GetComponent<BoxCollider2D>(), groundLayers);
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if(dead)
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

    public void HandleCollision(CollisionDetails collisionDetails)
    {
        if(collisionDetails.velocity.y < 0)
        {
            TakeDamage();
            collisionDetails.collisionObject.RecoilUp();
        }
        else if(collisionDetails.isAttack)
        {
            TakeDamage();
        }
        else
        {
            collisionDetails.collisionObject.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        animator.SetBool("Dead", true);
        velocity = new Vector2();
        transform.Translate(0, -0.15f, 0);
        GetComponent<BoxCollider2D>().enabled = false;
        dead = true;
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity *= -1;
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
