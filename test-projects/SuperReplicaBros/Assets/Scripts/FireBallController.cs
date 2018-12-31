using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour, ICollisionObject
{
    private float velocity = 15.0f;
    public bool facingRight = true;
    public float maxDistance = 30.0f;
    private float traveledDistance = 0.0f;

    private void Start()
    {
        if (!facingRight)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (traveledDistance > maxDistance)
        {
            Destroy(gameObject);
        }
        float step = velocity * Time.deltaTime;
        transform.Translate(step, 0f, 0f);
        traveledDistance += Mathf.Abs(step);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.SendMessage(
                "HandleCollision",
                new CollisionDetails { velocity = new Vector2(velocity, 0), collisionObject = this, isAttack = true }
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
