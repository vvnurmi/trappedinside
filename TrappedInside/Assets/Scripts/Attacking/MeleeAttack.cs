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
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(InputProvider))]
public class MeleeAttack : MonoBehaviour
{
    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterController2D characterController;
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
        characterController = GetComponent<CharacterController2D>();
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
        if (input.fire1)
            timedAnimTriggers.Set("StartMelee");
    }

    public void AnimEvent_StartAttacking(MeleeAttackType attack)
    {
        Debug.Assert(!activeAttack.HasValue);
        activeAttack = attack;

        switch (attack)
        {
            case MeleeAttackType.Sword:
            case MeleeAttackType.SwordSwingUp:
            case MeleeAttackType.ShieldThrow:
                characterController.state.isInHorizontalAttackMove = true;
                characterController.state.isInVerticalAttackMove = true;
                break;
            case MeleeAttackType.ShieldBash:
                characterController.state.isInVerticalAttackMove = true;
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
        Debug.Assert(activeAttack.HasValue);

        characterController.state.isInHorizontalAttackMove = false;
        characterController.state.isInVerticalAttackMove = false;
        DeactivateWeapons();
        animator.SetTrigger("StopMelee");
        activeAttack = null;
    }
}
