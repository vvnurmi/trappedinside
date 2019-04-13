using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Damages things that come in contact with gameObject.
/// </summary>
public class ContactAttack : MonoBehaviour
{
    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    [Tooltip("Cooldown for inflicting damge, in seconds.")]
    public float hitCooldown = 1.0f;

    // Modified during gameplay.
    private float nextHitAllowedTime;

    #region MonoBehaviour overrides

    private void OnTriggerEnter2D(Collider2D collision) => Hit(collision);
    private void OnTriggerStay2D(Collider2D collision) => Hit(collision);

    #endregion

    private void Hit(Collider2D collision)
    {
        if (nextHitAllowedTime > Time.time) return;

        var victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp == null) return;

        victimHp.Damage(hitDamage);
        nextHitAllowedTime = Time.time + hitCooldown;
    }
}
