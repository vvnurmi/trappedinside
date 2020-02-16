using UnityEngine;

public class ContactTrigger : MonoBehaviour
{
    public bool Contact { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Contact = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Contact = false;
    }
}
