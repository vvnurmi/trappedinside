using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelBehavior : MonoBehaviour
{
    private bool playerInProximity = false;
    private readonly float speed = 0.8f;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInProximity)
            return;

        transform.Translate(Time.deltaTime * new Vector3(0f, speed));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerInProximity = true;
            animator.Play("Squirrel running");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            playerInProximity = false;
            animator.Play("Squirrel idle");
        }
    }


    private bool IsPlayer(Collider2D collision) => collision.gameObject.tag == "Player";
}
