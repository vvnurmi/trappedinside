using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles melee attacking.
/// 
/// Melee is initiated by MeleeAttack and its phases are controlled by the
/// character's animation.
/// 
/// Usage tip: Each possible weapon is a subobject. Subobjects should have
/// a <see cref="MeleeHit"/> to identify the weapon type and its properties.
/// Subjects should also have a collider to represent the weapon's impact area.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterState))]
[RequireComponent(typeof(InputProvider))]
public class MeleeAttack : MonoBehaviour
{
    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterState characterState;
    private InputProvider inputProvider;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    // Modified during gameplay.
    MeleeAttackType? activeAttack;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterState = GetComponent<CharacterState>();
        DeactivateWeapons();
        inputProvider = GetComponent<InputProvider>();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        var input = inputProvider.GetInput();
        HandleInput(input);
    }

    #endregion

    private void DeactivateWeapons()
    {
        foreach (var weapon in GetComponentsInChildren<MeleeHit>())
            weapon.gameObject.SetActive(false);
    }

    private void ActivateWeapon(MeleeAttackType type)
    {
        foreach (var weapon in GetComponentsInChildren<MeleeHit>(includeInactive: true))
            if (weapon.attackType == type)
            {
                weapon.gameObject.SetActive(true);
                var attack = weapon.gameObject.GetComponent<IAttack>();
                attack?.OnAttack();
            }
    }

    private void HandleInput(PlayerInput input)
    {
        animator.SetBool("IsPrepUp", input.vertical > 0.5f);
        animator.SetBool("IsPrepDown", input.vertical < -0.5f);
        animator.SetBool("IsPrepSide", Math.Abs(input.horizontal) > 0.5f);
        animator.SetBool("WantsToShield", input.fire2Active && characterState.CanInflictDamage);
        if (characterState.CanInflictDamage)
        {
            if (input.fire1Pressed)
                timedAnimTriggers.Set("StartMelee");
            if (input.fire2Pressed)
                timedAnimTriggers.Set("StartShielding");
        }
    }

    public void AnimEvent_StartAttacking(MeleeAttackType attack)
    {
        if (activeAttack.HasValue)
        {
            if (activeAttack.Value == attack) return;
            StopAttacking(animatorAction: () => { });
        }

        activeAttack = attack;

        switch (attack)
        {
            case MeleeAttackType.ShieldUp:
            case MeleeAttackType.ShieldDiagonal:
            case MeleeAttackType.ShieldSide:
            case MeleeAttackType.ShieldThrow:
                SetCharacterHorzontalAndVerticalAttackState(true);
                break;
            case MeleeAttackType.ShieldBash:
                characterState.isInVerticalAttackMove = true;
                break;
            default:
                Debug.LogError($"Unhandled {nameof(MeleeAttackType)} {attack}");
                break;
        }
        ActivateWeapon(attack);
        audioSource.TryPlay(meleeSound);
    }

    public void AnimEvent_StopAttacking()
    {
        StopAttacking(() => animator.SetTrigger("StopMelee"));
    }

    private void StopAttacking(Action animatorAction)
    {
        Debug.Assert(activeAttack.HasValue);
        SetCharacterHorzontalAndVerticalAttackState(false);
        DeactivateWeapons();
        animatorAction();
        activeAttack = null;
    }

    private void SetCharacterHorzontalAndVerticalAttackState(bool state)
    {
        characterState.isInHorizontalAttackMove = state;
        characterState.isInVerticalAttackMove = state;
    }
}
