using UnityEngine;

/// <summary>
/// Makes a game object damageable.
/// </summary>
public class HitPoints : MonoBehaviour
{
    [Tooltip("Number of hit points when not damaged.")]
    public int maxHitPoints = 1;

    private int hitPoints;

    private void Start()
    {
        hitPoints = maxHitPoints;
    }

    public void Damage(int damage)
    {
        Debug.Assert(damage > 0);
        hitPoints = Mathf.Max(0, hitPoints - damage);

        if (hitPoints == 0)
        {
            Destroy(gameObject);
        }
    }
}
