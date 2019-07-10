using UnityEngine;

/// <summary>
/// When <see cref="HitPoints"/> signals death, do some UI tricks
/// and then restart the level.
/// </summary>
public class DeathRestartsLevel : MonoBehaviour, IDying
{
    public void OnDying()
    {
        // TODO: Wait for a couple of seconds first
        //!!!UIController.Instance.RestartLevel();
    }
}
