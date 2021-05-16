using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private GameObject player;

    public bool PlayerInAttackRange { get; private set; }
    public bool PlayerInLeft { get; private set; }
    public Vector3? PlayerPosition => player?.transform.position;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayer(collision))
        {
            PlayerInAttackRange = true;
            if (player == null)
                player = collision.gameObject;

            PlayerInLeft = player.transform.position.x < transform.position.x;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsPlayer(collision))
            PlayerInAttackRange = false;
    }

    private bool IsPlayer(Collider2D collision) => collision.CompareTag("Player");
}
