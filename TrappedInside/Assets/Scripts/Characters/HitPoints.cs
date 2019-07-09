using System;
using System.Collections;
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
public class HitPoints : MonoBehaviour
{
    [Tooltip("Number of hit points when not damaged.")]
    public int maxHitPoints = 1;

    [Tooltip("Seconds after hit until another hit is possible.")]
    public float hitDelay = 1.0f;

    public int CurrentHitPoints { get; private set; }

    // Set about once, probably in Start().
    private Animator animator; // May be null.

    // Modified during gameplay.
    private float nextHitAllowedAt = 0f;

    #region MonoBehaviour overrides

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
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
        CallHandlersOrDefault((Action<IDamaged>)(a => a.OnDamaged()), DefaultDamagedHandler);
        if (CurrentHitPoints == 0)
            Die();
    }

    private void Die()
    {
        // We're dead. Call death handlers if there's any.
        // If there's no handlers then just destroy the game object.
        CallHandlersOrDefault<IDying>(a => a.OnDying(), DefaultDyingHandler);
    }

    private void DefaultDamagedHandler()
    {
        animator?.Play("Damage");
    }

    private void DefaultDyingHandler()
    {
        animator?.Play("Death");
    }

    private void CallHandlersOrDefault<TInterface>(Action<TInterface> invoke, Action defaultt)
    {
        var handlers = GetComponents<TInterface>();
        if (handlers.Length > 0)
            foreach (var handler in handlers)
                invoke(handler);
        else
            defaultt();
    }
}
