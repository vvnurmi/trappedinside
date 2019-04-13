using System;
using UnityEngine;

/// <summary>
/// Handles melee attacking.
/// Assumes that gameObject has one CircleCollider. It's the hit area of the melee attack.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(InputProvider))]
public class MeleeAttack : MonoBehaviour
{
    [Tooltip("The sound to play on melee attack.")]
    public AudioClip meleeSound;

    [Tooltip("Amount of damage to inflict on hit.")]
    public int hitDamage = 1;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;
    private CharacterController2D characterController;
    private CircleCollider2D attackCollider;
    private InputProvider inputProvider;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController2D>();
        attackCollider = GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;
        inputProvider = GetComponent<InputProvider>();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        timedAnimTriggers.Update();

        UpdateAttackColliderPosition();

        var input = inputProvider.GetInput();
        HandleFireInput(input);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitPoints victimHp = collision.gameObject.GetComponent<HitPoints>();
        if (victimHp != null)
            victimHp.Damage(hitDamage);
    }

    #endregion

    /// <summary>
    /// Flips attack collider to the side the character is facing.
    /// </summary>
    private void UpdateAttackColliderPosition()
    {
        var oldOffset = attackCollider.offset;
        var shouldFlip = oldOffset.x > 0 != characterController.state.collisions.faceDir > 0;
        attackCollider.offset = new Vector2(
            shouldFlip ? -oldOffset.x : oldOffset.x,
            oldOffset.y);
    }

    private void HandleFireInput(PlayerInput input)
    {
        if (input.fire1)
        {
            timedAnimTriggers.Set("StartMelee");
            audioSource.PlayOneShot(meleeSound);
        }
    }

    public void AnimEvent_StartAttacking()
    {
        characterController.state.isInMelee = true;
        attackCollider.enabled = true;
    }

    public void AnimEvent_StopAttacking()
    {
        characterController.state.isInMelee = false;
        attackCollider.enabled = false;
        animator.SetTrigger("StopMelee");
    }
}
