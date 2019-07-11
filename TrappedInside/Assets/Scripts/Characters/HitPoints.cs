using System;
using UnityEngine;

/// <summary>
/// Interface for reacting to receiving damage.
/// </summary>
public interface IDamaged
{
    void OnDamaged();
}

/// <summary>
/// Interface for custom death handler.
/// </summary>
public interface IDying
{
    void OnDying();
}

/// <summary>
/// Makes a game object damageable.
/// </summary>
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController2D))]
public class HitPoints : MonoBehaviour
{
    [Tooltip("Number of hit points when not damaged.")]
    public int maxHitPoints = 1;

    [Tooltip("Seconds after hit until another hit is possible.")]
    public float hitDelay = 1.0f;

    [Tooltip("The sound to play on getting hit.")]
    public AudioClip hitSound;

    [Tooltip("The sound to play on dying.")]
    public AudioClip deathSound;

    public int CurrentHitPoints { get; private set; }

    // Set about once, probably in Start().
    private Animator animator; // May be null.
    private AudioSource audioSource;
    private CharacterController2D characterController;

    // Modified during gameplay.
    private float nextHitAllowedAt = 0f;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController2D>();
        CurrentHitPoints = maxHitPoints;
    }

    #endregion

    public void Damage(int damage)
    {
        Debug.Assert(damage > 0);
        if (CurrentHitPoints == 0) return;
        if (Time.time < nextHitAllowedAt) return;

        nextHitAllowedAt = Time.time + hitDelay;

        CurrentHitPoints = Mathf.Max(0, CurrentHitPoints - damage);
        CallHandlers<IDamaged>(a => a.OnDamaged());
        var sound = CurrentHitPoints > 0
            ? hitSound
            : deathSound;
        audioSource.TryPlay(sound);
        if (CurrentHitPoints == 0)
            Die();
    }

    private void Die()
    {
        characterController.state.isDead = true;
        animator.SetBool("IsDead", true);
        CallHandlers<IDying>(a => a.OnDying());
    }

    private void CallHandlers<TInterface>(Action<TInterface> invoke)
    {
        var handlers = GetComponents<TInterface>();
        foreach (var handler in handlers)
            invoke(handler);
    }
}
