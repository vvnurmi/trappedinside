using UnityEngine;

public class ContactTrigger : MonoBehaviour
{
    public bool ignoreDefaultLayer = false;

    public bool Contact { get; private set; }

    private int DefaultLayer => LayerMask.NameToLayer("Default");

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != DefaultLayer)
            Contact = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer != DefaultLayer)
            Contact = false;
    }
}
