using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    [Tooltip("How many characters to type per second")]
    public float charsPerSecond = 10;

    // Set about once, probably in Start().
    private Text textComponent;
    private string fullText;
    private float startTime;

    // Modified during gameplay.
    private int charsToShow;

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
        ReadInput();

        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * charsPerSecond),
            min: charsToShow,
            max: fullText.Length);
        textComponent.text = fullText.Substring(0, charsToShow);
    }

    #endregion

    private void ReadInput()
    {
        var isSubmitDown = Input.GetButtonDown("Submit");
        if (isSubmitDown)
        {
            // First reveal all of the text. If that's already the case
            // then close the owning narrative box.
            if (charsToShow < fullText.Length)
                charsToShow = fullText.Length;
            else
                gameObject.SetActive(false);
        }
    }
}
