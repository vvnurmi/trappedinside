using System.Linq;
using UnityEngine;

public class NarrativeTypistSetup
{
    public string fullText;
    public string speaker;
    public string[] choices;
}

public enum NarrativeTypistState
{
    Uninitialized,
    Typing,
    UserPrompt,
    Finished,
}

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    // Set about once, probably in Start().
    protected NarrativeTypistSettings settings;
    protected ITIInputContext inputContext;
    private TMPro.TextMeshProUGUI textComponent;
    private NarrativeTypistSetup setup;
    private float startTime;

    // Modified during gameplay.
    private int charsToShow;

    public NarrativeTypistState State { get; set; } = NarrativeTypistState.Uninitialized;

    /// <summary>
    /// Starts the typing process. The process will finish when all
    /// of <paramref name="fullText"/> is displayed.
    /// </summary>
    public virtual void StartTyping(NarrativeTypistSetup narrativeTypistSetup)
    {
        State = NarrativeTypistState.Typing;
        setup = narrativeTypistSetup;
        charsToShow = 0;
        startTime = Time.time;

        var textFields = GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        foreach (var textField in textFields)
        {
            if (textField.gameObject.CompareTag(TiaSpeech.TagText))
            {
                textComponent = textField;
                textField.text = "";
            }
            if (textField.gameObject.CompareTag(TiaSpeech.TagSpeaker))
                textField.text = setup.speaker;
        }
    }

    #region MonoBehaviour overrides

    private void Start()
    {
        Debug.Assert(inputContext == null);
        inputContext = TIInputStateManager.instance.CreateContext();
    }

    private void OnDestroy()
    {
        inputContext?.Dispose();
    }

    protected virtual void Awake()
    {
        settings = GetComponentInParent<NarrativeTypistSettings>();
        Debug.Assert(settings != null,
            $"Expected to find {nameof(NarrativeTypistSettings)} from the parent of {nameof(NarrativeTypist)}");
        textComponent = GetComponentsInChildren<TMPro.TextMeshProUGUI>()
            .Single(text => text.gameObject.CompareTag(TiaSpeech.TagText));
    }

    protected virtual void FixedUpdate()
    {
        if (State == NarrativeTypistState.Uninitialized) return;
        if (State == NarrativeTypistState.Finished) return;

        var inputState = inputContext.GetStateAndResetEventFlags();
        HandleInput(inputState);

        UpdateAudiovisuals();

        if (State == NarrativeTypistState.Typing && textComponent.text.Length == setup.fullText.Length)
            OnTypingFinished();
    }

    #endregion

    /// <summary>
    /// Called when typing has finished but has not yet been acknowledged.
    /// </summary>
    protected virtual void OnTypingFinished()
    {
        State = NarrativeTypistState.UserPrompt;
        charsToShow = setup.fullText.Length;
    }

    /// <summary>
    /// Called when typing has finished and the player has acknowledged it.
    /// </summary>
    protected virtual void OnTypingAcknowledged()
    {
        State = NarrativeTypistState.Finished;
    }

    private void HandleInput(TIInputState inputState)
    {
        if (inputState.uiSubmitPressed)
        {
            // First reveal all of the text. If that's already the case
            // then "acknowledge" the dialog box and move on.
            switch (State)
            {
                case NarrativeTypistState.Typing:
                    OnTypingFinished();
                    break;
                case NarrativeTypistState.UserPrompt:
                    OnTypingAcknowledged();
                    break;
            }
        }
    }

    private void UpdateAudiovisuals()
    {
        var oldCharsToShow = textComponent.text.Length;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: setup.fullText.Length);
        if (charsToShow == oldCharsToShow) return;

        // Visual update.
        textComponent.text = setup.fullText.Substring(0, charsToShow);

        // Audio update.
        var lastCharIsSpace = textComponent.text.Length == 0 ||
            char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
        if (!lastCharIsSpace)
            settings.audioSource.TryPlay(settings.characterSound);
    }
}
