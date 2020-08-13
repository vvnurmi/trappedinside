using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
public class MeleeAttack : MonoBehaviour
{
    [EnumFlag("Capabilities")]
    [Tooltip("What kind of attacks are enabled.")]
    public MeleeAttackCapabilities capabilities;

    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterState characterState;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    // Modified during gameplay.
    private MeleeAttackType? activeAttack;
    private TIInputStateManager inputStateManager = new TIInputStateManager();

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterState = GetComponent<CharacterState>();
        DeactivateWeapons();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        RelayWeaponCapabilities();

        var currentInput = inputStateManager.GetStateAndResetEventFlags();
        HandleInput(currentInput);
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

    private void RelayWeaponCapabilities()
    {
        foreach (MeleeAttackCapabilities capability in Enum.GetValues(typeof(MeleeAttackCapabilities)))
            if (capability != MeleeAttackCapabilities.None)
                animator.SetBool(capability.ToString(), capabilities.HasFlag(capability));
    }

    private void HandleInput(TIInputState input)
    {
        animator.SetBool("IsPrepUp", input.vertical > 0.5f && characterState.CanInflictDamage);
        animator.SetBool("IsPrepDown", input.vertical < -0.5f && characterState.CanInflictDamage);
        animator.SetBool("IsPrepSide", Math.Abs(input.horizontal) > 0.5f && characterState.CanInflictDamage);
        animator.SetBool("WantsToShield", input.fire2Active && characterState.CanInflictDamage);
        if (characterState.CanInflictDamage)
        {
            if (input.fire1Pressed)
                timedAnimTriggers.Set("StartMelee");
            if (input.fire2Pressed)
                timedAnimTriggers.Set("StartShielding");
        }
    }

    public void InputEvent_Move(InputAction.CallbackContext context) =>
        inputStateManager.InputEvent_Move(context);

    public void InputEvent_Shield(InputAction.CallbackContext context) =>
        inputStateManager.InputEvent_Shield(context);

    public void AnimEvent_StartAttacking(MeleeAttackType attack)
    {
        var playMeleeSound = true;
        if (activeAttack.HasValue)
        {
            if (activeAttack.Value == attack) return;
            StopAttacking(animatorAction: () => { });
            playMeleeSound = false;
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

        if (playMeleeSound)
            audioSource.TryPlay(meleeSound);
    }

    public void AnimEvent_StopAttacking()
    {
        StopAttacking(() => animator.SetTrigger("StopMelee"));
    }

    private void StopAttacking(Action animatorAction)
    {
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
