using UnityEngine;

/// <summary>
/// When <see cref="HitPoints"/> signals death, do some UI tricks
/// and then restart the level.
/// </summary>
public class DeathRestartsLevel : MonoBehaviour, IDying
{
    [Tooltip("Dialog box for displaying game over text.")]
    public GameObject gameOverBox;

    public void OnDying()
    {
        var canvas = FindObjectOfType<Canvas>();
        Debug.Assert(canvas != null, $"Couldn't find a {nameof(Canvas)} to create a game over box");
        Instantiate(gameOverBox, canvas.transform);
    }
}
