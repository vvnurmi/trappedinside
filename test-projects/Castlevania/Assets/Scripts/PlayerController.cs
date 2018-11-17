﻿using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float speed = 7f;
    private Rigidbody2D rb2d;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D boxCollider2d;
    private bool facingRight = true;
    private bool grounded = false;
    public Transform groundCheck;
    private float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public float jumpForce = 5.0f;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate() {
        var verticalInput = Input.GetAxis("Vertical");
        if (verticalInput < -0.5) {
            animator.SetBool("Crouching", true);
            boxCollider2d.size = new Vector2(0.4f, 0.5f);
            return;
        }
        else {
            boxCollider2d.size = new Vector2(0.4f, 1.0f);
            animator.SetBool("Crouching", false);
            grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
            var horizontalInput = Input.GetAxis("Horizontal");

            if (grounded && verticalInput > 0) {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }

            animator.SetBool("Walking", Math.Abs(horizontalInput) != 0.0f && grounded);
            rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);

            if (horizontalInput < 0 && facingRight) {
                Flip();
            }
            else if (horizontalInput > 0 && !facingRight) {
                Flip();
            }
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
    }
}
