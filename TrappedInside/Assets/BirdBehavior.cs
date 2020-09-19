using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    private bool disturbed = false;
    private float timeWhenDisturbed = -1f;
    private Vector3 speed = new Vector3(-1.0f, 1.5f);
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!disturbed)
            return;

        // Destroy game object when far enough
        if (Time.time > timeWhenDisturbed + 10.0f)
            Destroy(gameObject);

        animator.SetBool("IsFlying", true);
        transform.Translate(speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            disturbed = true;
            timeWhenDisturbed = Time.time;
        }
    }
}
