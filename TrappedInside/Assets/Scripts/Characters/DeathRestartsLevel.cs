using System.Collections;
using UnityEngine;

/// <summary>
/// When <see cref="HitPoints"/> signals death, do some UI tricks
/// and then restart the level.
/// </summary>
public class DeathRestartsLevel : MonoBehaviour, IDying
{
    [Tooltip("Restart level after this many seconds have passed since death.")]
    public float secondsUntilLevelRestart = 5;

    public void OnDying()
    {
        StartCoroutine(DyingHandler());
    }

    private IEnumerator DyingHandler()
    {
        yield return new WaitForSeconds(secondsUntilLevelRestart);
        UIController.Instance.RestartLevel();
    }
}
