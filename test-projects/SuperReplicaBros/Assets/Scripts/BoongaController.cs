using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoongaController : MonoBehaviour {

    public float velocity = -2f;
    public Transform wallCheck;
    public LayerMask whatIsGround;

    private Rigidbody2D rb2d;
    private float wallCheckRadius = 0.15f;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (transform.position.y < -100)
            Destroy(gameObject);

        transform.Translate(velocity * Time.deltaTime, 0, 0);

        if(HitWall()) {
            Flip();
        }
    }

    bool HitWall() {
        return Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, whatIsGround);
    }

    private void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        velocity *= -1;
    }


    // Update is called once per frame
    void Update () {
    }
}
