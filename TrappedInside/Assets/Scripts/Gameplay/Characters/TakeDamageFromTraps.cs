using UnityEngine;

public class TakeDamageFromTraps : MonoBehaviour
{
    private HitPoints hitPoints;

    void Start()
    {
        hitPoints = GetComponent<HitPoints>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsTrap(collision))
        {
            hitPoints.Damage(1);
        }
    }

    private bool IsTrap(Collider2D collision) =>
        collision.gameObject.layer == LayerMask.NameToLayer("Traps");

}
