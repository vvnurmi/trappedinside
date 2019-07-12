using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// When <see cref="HitPoints"/> signals death, do some UI tricks
/// and then restart the level.
/// </summary>
public class DeathRestartsLevel : MonoBehaviour, IDying
{
    [Tooltip("Restart level after this many seconds have passed since death.")]
    public float secondsUntilLevelRestart = 5;

    [Tooltip("Dialog box for displaying game over text.")]
    public GameObject gameOverBox;

    [Tooltip("Quips to display in game over box.")]
    [TextArea]
    public string[] gameOverQuips;

    public void OnDying()
    {
        StartCoroutine(DyingHandler());
    }

    private IEnumerator DyingHandler()
    {
        var canvas = GameObject.FindGameObjectWithTag("Canvas");
        Debug.Assert(canvas != null);
        Instantiate(gameOverBox, canvas.transform);
        var quip = GameObject.FindGameObjectWithTag("GameOverQuip");
        Debug.Assert(quip != null);
        var quipText = quip.GetComponent<Text>();
        quipText.text = gameOverQuips[Random.Range(0, gameOverQuips.Length)];

        yield return new WaitForSeconds(secondsUntilLevelRestart);

        UIController.Instance.RestartLevel();
    }
}
