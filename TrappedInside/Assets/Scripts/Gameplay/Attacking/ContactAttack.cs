﻿using UnityEngine;

/// <summary>
/// Damages things that come in contact with gameObject.
/// </summary>
public class ContactAttack : MonoBehaviour
{
    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    [Tooltip("Amount of damage to self on hit")]
    public bool damagedSelfOnHit = false;

    // Set about once, probably in Start().
    private CharacterState characterState; // May be null.

    #region MonoBehaviour overrides

    private void Start()
    {
        characterState = GetComponentInParent<CharacterState>();
    }

    private void OnTriggerEnter2D(Collider2D collision) => Hit(collision);
    private void OnTriggerStay2D(Collider2D collision) => Hit(collision);

    #endregion

    private void Hit(Collider2D collision)
    {
        if (characterState?.CanInflictDamage == false) return;

        var victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp == null) return;
        victimHp.Damage(hitDamage);

        if (damagedSelfOnHit)
        {
            var ownHp = gameObject.GetComponent<HitPoints>();
            if (ownHp == null) return;
            ownHp.Damage(hitDamage);
        }
    }
}
