using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    public bool PlayerInAttackRange { get; private set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInAttackRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInAttackRange = false;
    }

    private bool IsPlayer(Collider2D collision) => collision.CompareTag("Player");
}
