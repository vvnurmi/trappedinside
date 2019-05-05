using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    // Set about once, probably in Start().
    private NarrativeTypistSettings settings;
    private Text textComponent;
    private string fullText;
    private float startTime;

    // Modified during gameplay.
    private int charsToShow;

    #region MonoBehaviour overrides

    private void Awake()
    {
        settings = GetComponentInParent<NarrativeTypistSettings>();
        Debug.Assert(settings != null,
            $"Expected to find {nameof(NarrativeTypistSettings)} from the parent of {nameof(NarrativeTypist)}");
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

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: fullText.Length);
        textComponent.text = fullText.Substring(0, charsToShow);

        var lastCharIsSpace = textComponent.text.Length == 0 ||
            char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
        if (oldCharsToShow < charsToShow && !lastCharIsSpace)
            settings.audioSource.PlayOneShot(settings.characterSound);
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
