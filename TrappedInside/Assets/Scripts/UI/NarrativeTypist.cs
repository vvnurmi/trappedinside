using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    [Tooltip("How many characters to type per second")]
    public float charsPerSecond = 10;

    private Text textComponent;
    private string fullText;
    private float startTime;

    #region MonoBehaviour overrides

    private void Awake()
    {
        textComponent = GetComponentInChildren<Text>();
        Debug.Assert(textComponent != null);
        fullText = textComponent.text;
    }

    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void FixedUpdate()
    {
        var charsToShow = Mathf.Min(
            fullText.Length,
            Mathf.RoundToInt((Time.time - startTime) * charsPerSecond));
        textComponent.text = fullText.Substring(0, charsToShow);

        // If all of the text is revealed, go to sleep as there's nothing more to do.
        if (charsToShow == fullText.Length)
            gameObject.SetActive(false);
    }

    #endregion
}
