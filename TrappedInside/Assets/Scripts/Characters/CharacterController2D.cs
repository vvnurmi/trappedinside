using System;
using UnityEngine;

/// <summary>
/// Handles walking character movement. Gravity is handcrafted.
/// </summary>
//[RequireComponent(typeof(Animator))] // Required on a child object
[RequireComponent(typeof(AudioSource))]
public class CharacterController2D : MonoBehaviour
{
    [Tooltip("Minimum time between two consecutive hits.")]
    public float hitDelay = 1.0f;

    public AudioClip hitSound;

    // Set about once, probably in Start().
    private Animator animator;
    private AudioSource audioSource;

    // Modified during gameplay.
    public CharacterState state = new CharacterState();
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
        animator = GetComponentInChildren<Animator>();
        Debug.Assert(animator != null);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (IsDead)
        {
            // TODO: Disable other behaviours
            return;
        }
    }

    #endregion

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
        audioSource.PlayOneShot(hitSound);
        nextHitAllowedAt = Time.time + hitDelay;
    }

    private void OnDeath()
    {
        animator.Play("Death");
        audioSource.PlayOneShot(hitSound);
        Death?.Invoke();
    }
}
