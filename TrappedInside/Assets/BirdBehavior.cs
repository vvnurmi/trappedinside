using UnityEngine;

public class BirdBehavior : MonoBehaviour
{
    private bool disturbed = false;
    private float timeWhenDisturbed = float.MinValue;
    private Animator animator;
    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        originalPosition = transform.position;
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
        var newX = timeWhenDisturbed - Time.time;
        var newY = 0.5f * Mathf.Pow(newX, 2f) + 0.25f * newX;
        transform.position = originalPosition + new Vector3(newX, newY);
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
