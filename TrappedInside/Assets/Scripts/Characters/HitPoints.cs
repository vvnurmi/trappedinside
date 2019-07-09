using UnityEngine;

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
    public int CurrentHitPoints { get; private set; }

    private void Start()
    {
        CurrentHitPoints = maxHitPoints;
    }

    public void Damage(int damage)
    {
        Debug.Assert(damage > 0);
        CurrentHitPoints = Mathf.Max(0, CurrentHitPoints - damage);
        if (CurrentHitPoints == 0)
            Die();
    }

    private void Die()
    {
        // We're dead. Call death handlers if there's any.
        // If there's no handlers then just destroy the game object.

        var handlers = GetComponents<IDying>();
        if (handlers.Length > 0)
            foreach (var handler in handlers)
                handler.OnDying();
        else
            Destroy(gameObject);
    }
}
