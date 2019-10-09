using UnityEngine;

public class ProximityTrigger : MonoBehaviour
{
    public bool PlayerInProximity { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInProximity = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInProximity = false;
    }

    private bool IsPlayer(Collider2D collision) => collision.CompareTag("Player");
}
