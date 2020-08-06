using System.Linq;
using UnityEngine;

public class NarrativeTypistSetup
{
    public string fullText;
    public string speaker;
    public string leftChoice;
    public string rightChoice;
}

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    // Set about once, probably in Start().
    private NarrativeTypistSettings settings;
    private TMPro.TextMeshProUGUI textComponent;
    private NarrativeTypistSetup setup;
    private float startTime;

    // Modified during gameplay.
    private int charsToShow;

    public bool IsDoneTyping => charsToShow == setup.fullText.Length;

    /// <summary>
    /// Starts the typing process. The process will finish when all
    /// of <paramref name="fullText"/> is displayed.
    /// </summary>
    public virtual void StartTyping(NarrativeTypistSetup setup)
    {
        this.setup = setup;
        charsToShow = 0;
        startTime = Time.time;

        var textFields = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (var textField in textFields)
        {
            if (textField.gameObject.CompareTag("SpeechText"))
            {
                textComponent = textField;
                textField.text = "";
            }
            if (textField.gameObject.CompareTag("SpeechSpeaker"))
                textField.text = setup.speaker;
        }
    }

    #region MonoBehaviour overrides

    virtual protected void Awake()
    {
        settings = GetComponentInParent<NarrativeTypistSettings>();
        Debug.Assert(settings != null,
            $"Expected to find {nameof(NarrativeTypistSettings)} from the parent of {nameof(NarrativeTypist)}");
        textComponent = GetComponentsInChildren<TMPro.TextMeshProUGUI>()
            .Single(text => text.gameObject.CompareTag("SpeechText"));
    }

    virtual protected void FixedUpdate()
    {
        if (setup == null) return;

        ReadInput();

        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: setup.fullText.Length);
        textComponent.text = setup.fullText.Substring(0, charsToShow);

        // If something more was typed, make noise and react to text end.
        if (oldCharsToShow < charsToShow)
        {
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace)
                settings.audioSource.TryPlay(settings.characterSound);
            if (IsDoneTyping)
                OnTypingFinished();
        }
    }

    #endregion

    /// <summary>
    /// Called when typing has finished but has not yet been acknowledged.
    /// </summary>
    virtual protected void OnTypingFinished()
    {
        charsToShow = setup.fullText.Length;
    }

    /// <summary>
    /// Called when typing has finished and the player has acknowledged it.
    /// </summary>
    virtual protected void OnTypingAcknowledged()
    {
        gameObject.SetActive(false);
    }

    private void ReadInput()
    {
        var isSubmitDown = Input.GetButtonDown("Submit");
        if (isSubmitDown)
        {
            // First reveal all of the text. If that's already the case
            // then "acknowledge" the dialog box and move on.
            if (!IsDoneTyping)
                OnTypingFinished();
            else
                OnTypingAcknowledged();
        }
    }
}
