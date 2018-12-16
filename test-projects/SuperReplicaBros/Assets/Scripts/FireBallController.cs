using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour, ICollisionObject {


    public Vector2 velocity = new Vector2(10f, 0);
    public bool facingRight = true;

	private void Start () {
		if(!facingRight)
        {
            Flip();
        }
	}

    private void FixedUpdate()
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.SendMessage(
                "HandleCollision",
                new CollisionDetails { velocity = velocity, collisionObject = this, isAttack = true }
                );
        }
        Destroy(gameObject);
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    public void RecoilUp()
    {
        throw new System.NotImplementedException();
    }

    public void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity *= -1;
    }
}
