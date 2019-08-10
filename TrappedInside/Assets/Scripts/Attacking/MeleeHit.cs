using UnityEngine;

/// <summary>
/// Handles the collision of a melee weapon to its target.
/// </summary>
public class MeleeHit : MonoBehaviour
{
    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    /// <summary>
    /// The activation mechanism is in <see cref="MeleeAttack"/>.
    /// </summary>
    [Tooltip("What kind of attack will activate this game object.")]
    public MeleeAttackType attackType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitPoints victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp != null)
            victimHp.Damage(hitDamage);
    }
}
