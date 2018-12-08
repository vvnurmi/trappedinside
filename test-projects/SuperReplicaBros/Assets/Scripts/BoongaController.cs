using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoongaController : MonoBehaviour {

    public float velocity = -2f;
    public Transform wallCheck;
    public LayerMask whatIsGround;
    public bool dead = false;

    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallCheckRadius = 0.15f;
    private bool deathHandled = false;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {

        if(dead && deathHandled) {
            return;
        }

        if(dead && !deathHandled) {
            HandleDeath();
            return;
        }

        if (transform.position.y < -100)
            Destroy(gameObject);

        transform.Translate(velocity * Time.deltaTime, 0, 0);

        if(HitWall()) {
            Flip();
        }
    }

    void HandleDeath() {
        animator.SetBool("Dead", true);
        velocity = 0;
        boxCollider.isTrigger = true;
        rb2d.isKinematic = true;
        transform.Translate(0, -0.15f, 0);
        deathHandled = true;
    }

    bool HitWall() {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsGround);
    }

    void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity *= -1;
    }

    void Destroy() {
        Destroy(gameObject);
    }
}
