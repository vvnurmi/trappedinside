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

    private CharacterState characterState;
    private float hitStartTime = 0.0f;
    private float hitTime = 0.0f;

    private void Start()
    {
        characterState = GetComponentInParent<CharacterState>();
        Debug.Assert(characterState != null);
    }

    private void FixedUpdate()
    {
        characterState.isInShieldingRecoil = Time.time < hitStartTime + hitTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var shieldContactBehaviour = collision.gameObject.GetComponent<ShieldContactBehaviour>();

        if(shieldContactBehaviour == null)
            return;

        if(shieldContactBehaviour.hasMomentum)
        {
            hitStartTime = Time.time;
            MomentumToHitTime(shieldContactBehaviour.momentum);
        }

        switch (shieldContactBehaviour.behaviour)
        {
            case ShieldContactBehaviour.Behaviour.TakesDamage:
            {
                var victimHp = collision.gameObject.GetComponent<HitPoints>();
                Debug.Assert(victimHp != null);
                victimHp.Damage(hitDamage);
                break;
            }
            case ShieldContactBehaviour.Behaviour.TurnsAround:
            {
                var flippable = collision.gameObject.GetComponent<IFlippable>();
                Debug.Assert(flippable != null);
                flippable.Flip();
                break;
            }
        }
    }

    private void MomentumToHitTime(float victimMomentum)
    {
        hitTime = 0.1f * victimMomentum;
    }
}
