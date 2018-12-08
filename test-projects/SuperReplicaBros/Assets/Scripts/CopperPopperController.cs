using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopperPopperController : MonoBehaviour {
    public float velocity = -2f;
    public Transform wallCheck;
    public bool cocoon = false;

    private Animator animator;
    private bool cocoonHandled = false;

    // Use this for initialization
    void Start() {
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

        if (transform.position.y < -100)
            Destroy(gameObject);

        Move();

    }

    private void Move() {
        transform.Translate(velocity * Time.deltaTime, 0, 0);
    }

    void HandleCocoon() {
        animator.SetBool("Cocoon", true);
        velocity = 0;
        cocoonHandled = true;
    }

    void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity *= -1;
        Move();
    }

    void Destroy() {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision) {
        Flip();    
    }
}
