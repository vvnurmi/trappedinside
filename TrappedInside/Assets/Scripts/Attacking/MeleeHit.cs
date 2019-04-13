using UnityEngine;

/// <summary>
/// Handles the collision of a melee weapon to its target.
/// </summary>
public class MeleeHit : MonoBehaviour
{
    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitPoints victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp != null)
            victimHp.Damage(hitDamage);
    }
}
