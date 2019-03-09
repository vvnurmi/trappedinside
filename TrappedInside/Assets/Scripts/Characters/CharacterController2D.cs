using System;
using UnityEngine;

/// <summary>
/// Handles walking character movement. Gravity is handcrafted.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputProvider))]
public class CharacterController2D : MonoBehaviour
{
    [Tooltip("Minimum time between two consecutive hits.")]
    public float hitDelay = 1.0f;

    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioClip punchSound;

    // Set about once, probably in Start().
    private Animator animator;
    private CircleCollider2D attackCollider;
    private AudioSource audioSource;
    private InputProvider inputProvider;

    // Helpers
    private TimedAnimationTriggers timedAnimTriggers;

    // Modified during gameplay.
    public CharacterState state;
    private float nextHitAllowedAt = 0f;
    public int health = 5;

    /// <summary>
    /// Invoked when the character dies.
    /// </summary>
    public event Action Death;

    private bool IsDead { get { return health <= 0; } }

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponent<Animator>();
        attackCollider = GetComponent<CircleCollider2D>();
        attackCollider.enabled = false;
        audioSource = GetComponent<AudioSource>();
        inputProvider = GetComponent<InputProvider>();

        timedAnimTriggers = new TimedAnimationTriggers(animator, 0.1f);
    }

    private void Update()
    {
        if (IsDead)
        {
            return;
        }

        timedAnimTriggers.Update();

        var input = inputProvider.GetInput();
        HandleFireInput(input);
    }

    #endregion

    public void Flip()
    {
        transform.localScale.Set(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void HandleFireInput(PlayerInput input)
    {
        if (input.fire1)
        {
            timedAnimTriggers.Set("StartMelee");
            PlaySound(punchSound);
        }
    }

    public void TakeDamage()
    {
        if (IsDead || Time.time < nextHitAllowedAt)
            return;

        health--;

        if (IsDead)
            OnDeath();
        else
            OnDamage();
    }

    public void KillInstantly()
    {
        if (IsDead) return;
        health = 0;
        OnDeath();
    }

    private void OnDamage()
    {
        animator.Play("Damage");
        PlaySound(hitSound);
        nextHitAllowedAt = Time.time + hitDelay;
    }

    private void OnDeath()
    {
        animator.Play("Death");
        PlaySound(hitSound);
        Death?.Invoke();
    }

    private void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            audioSource.PlayOneShot(sound);
        }
    }

    public void AnimEvent_StartAttacking()
    {
        state.isInMelee = true;
        attackCollider.enabled = true;
    }

    public void AnimEvent_StopAttacking()
    {
        state.isInMelee = false;
        attackCollider.enabled = false;
        animator.SetTrigger("StopMelee");
    }
}
