using UnityEngine;

public class MedusaController : MonoBehaviour {

    private Rigidbody2D rb2d;
    public float speed = 1.0f;
    private readonly float maxDistance = 9.0f;
    private bool facingRight = true;
    public float movementFrequency = 1.0f;
    public float movementAmplitude = 2.0f;
    public int health = 3;
    private Vector3 startingPosition;

    void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = new Vector2(speed, 0);
        startingPosition = transform.position;
    }

    void FixedUpdate () {
        rb2d.position = new Vector2(rb2d.position.x, startingPosition.y + movementAmplitude * Mathf.Sin(2.0f * Mathf.PI * movementFrequency * Time.time));

        if (transform.position.x < -maxDistance) {
            Flip();
        }
        else if(transform.position.x > maxDistance) {
            Flip();
        }
    }

    private void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        rb2d.velocity = new Vector3(-rb2d.velocity.x, 0);
    }
}
