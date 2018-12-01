using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 7f;
    private Rigidbody2D rb2d;
    private Animator animator;
    private BoxCollider2D boxCollider2d;
    private AudioSource audioSource;
    private bool facingRight = true;
    private bool grounded = false;
    public Transform groundCheck;
    public GameObject whip;
    private readonly float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    public float jumpForce = 5.0f;
    public float health = 10.0f;
    private bool crouchPressed = false;
    public float enemyHitForce = 2f;

    public AudioClip whipSound;



    private HitController hitController;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider2d = GetComponent<BoxCollider2D>();
        hitController = GetComponent<HitController>();
        audioSource = GetComponent<AudioSource>();
        DeactivateWhip();
	}
	
	// Update is called once per frame
	void Update () {
    }

    void FixedUpdate() {


        if (hitController.PlayerImmobile) {
            return;
        }

        var jumpPressedInThisFrame = Input.GetKeyDown(KeyCode.UpArrow);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            crouchPressed = true;
        else if(Input.GetKeyUp(KeyCode.DownArrow))
            crouchPressed = false;

        Attack(false);

        if (crouchPressed) {
            SetCrouchAnimation(true);
            boxCollider2d.size = new Vector2(0.4f, 0.5f);
        }
        else {
            SetCrouchAnimation(false);
            boxCollider2d.size = new Vector2(0.4f, 1.0f);

            if (AttackPressed) {
                audioSource.PlayOneShot(whipSound);
                Attack(true);
            }

            grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);


            if (grounded && jumpPressedInThisFrame) {
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
        var whipController = whip.GetComponent<WhipController>();
        if(whipController.IsWhipCollision) {
            whipController.IsWhipCollision = false;
            return;
        }

        if(collision.gameObject.CompareTag("Enemy") && !hitController.HitEffectOn) {
            hitController.HandleCollision(rb2d);
            health -= 1;
        }
        else if(collision.gameObject.CompareTag("Ham")) {
            health += 10;
            Destroy(collision.gameObject);
        }
    }
}
