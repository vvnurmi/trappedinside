using UnityEngine;

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
        {
            Destroy(gameObject);
        }
    }
}
