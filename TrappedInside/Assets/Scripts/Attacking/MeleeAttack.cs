﻿using System.Collections.Generic;
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
        HandleFireInput(input);
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
                weapon.gameObject.SetActive(true);
    }

    private void HandleFireInput(PlayerInput input)
    {
        if (input.fire1)
            timedAnimTriggers.Set("StartMelee");
    }

    public void AnimEvent_StartAttacking(MeleeAttackType attack)
    {
        switch (attack)
        {
            case MeleeAttackType.Sword:
                characterController.state.isInHorizontalAttackMove = true;
                characterController.state.isInVerticalAttackMove = true;
                break;
            case MeleeAttackType.ShieldBash:
                characterController.state.isInVerticalAttackMove = true;
                break;
        }
        ActivateWeapon(attack);
        audioSource.TryPlay(meleeSound);
    }

    public void AnimEvent_StopAttacking()
    {
        characterController.state.isInHorizontalAttackMove = false;
        characterController.state.isInVerticalAttackMove = false;
        DeactivateWeapons();
        animator.SetTrigger("StopMelee");
    }
}
