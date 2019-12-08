using UnityEngine;

/// <summary>
/// Damages things that come in contact with gameObject.
/// </summary>
public class ContactAttack : MonoBehaviour
{
    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    // Set about once, probably in Start().
    private CharacterController2D characterController; // May be null.

    #region MonoBehaviour overrides

    private void Start()
    {
        characterController = GetComponentInParent<CharacterController2D>();
        Debug.Assert(characterController != null, $"{nameof(CharacterController2D)} not found in parents of {name}");
    }

    private void OnTriggerEnter2D(Collider2D collision) => Hit(collision);
    private void OnTriggerStay2D(Collider2D collision) => Hit(collision);

    #endregion

    private void Hit(Collider2D collision)
    {
        if (characterController?.state?.CanInflictDamage == false) return;

        var victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp == null || collision.gameObject.layer == LayerMask.NameToLayer("Enemy")) return;

        victimHp.Damage(hitDamage);
    }
}
