using UnityEngine;

public class AlertTrigger : MonoBehaviour
{
    public bool PlayerInVisionRange { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInVisionRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInVisionRange = false;
    }

    private bool IsPlayer(Collider2D collision) => collision.CompareTag("Player");

}
