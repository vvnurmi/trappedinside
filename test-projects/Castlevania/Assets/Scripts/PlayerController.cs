using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float speed = 7f;
    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D boxCollider2d;
    private bool facingRight = true;
    private bool grounded = false;
    public Transform groundCheck;
    public GameObject whip;
    private readonly float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public float jumpForce = 5.0f;
    public float health = 10.0f;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        whip.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate() {
        var verticalInput = Input.GetAxis("Vertical");
        Attack(false);

        if (verticalInput < -0.5) {
            SetCrouchAnimation(true);
            boxCollider2d.size = new Vector2(0.4f, 0.5f);
        }
        else {
            SetCrouchAnimation(false);
            boxCollider2d.size = new Vector2(0.4f, 1.0f);

            if (AttackPressed) {
                Attack(true);
            }

            grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
            if (grounded && verticalInput > 0) {
                rb2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            }

            var horizontalInput = Input.GetAxis("Horizontal");
            SetWalkAnimation(Math.Abs(horizontalInput) != 0.0f && grounded);
            rb2d.velocity = new Vector2(horizontalInput * speed, rb2d.velocity.y);

            if (horizontalInput < 0 && facingRight) {
                Flip();
            }
            else if (horizontalInput > 0 && !facingRight) {
                Flip();
            }
        }
    }

    private bool AttackPressed {
        get {
            return Input.GetKeyDown(KeyCode.RightControl);
        }
    }

    private void ActivateWhip() {
        whip.SetActive(true);
    }

    private void DeactivateWhip() {
        whip.SetActive(false);
    }

    private void Attack(bool value) {
        animator.SetBool("Attacking", value);
    }

    private void SetCrouchAnimation(bool value) {
        animator.SetBool("Crouching", value);
    }

    private void SetWalkAnimation(bool value) {
        animator.SetBool("Walking", value);
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        rb2d.velocity = new Vector3(-rb2d.velocity.x, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Enemy")) {
            health -= 1;
        }
    }
}
