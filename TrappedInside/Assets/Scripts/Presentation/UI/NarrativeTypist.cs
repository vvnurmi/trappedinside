using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Displays text in a text box as if it was being typed in.
/// </summary>
public class NarrativeTypist : MonoBehaviour
{
    public NarrativeTypistSettings settings;

    /// <summary>
    /// Internal state of <see cref="CalculateRichTextLengths(string)"/>.
    /// </summary>
    private enum RichTextParseState { Text, InTag, AfterTag };

    // Set about once, probably in Start().
    protected AudioSource audioSource;
    protected ITIInputContext inputContext;
    private SpeechBubbleController speechBubble;
    private NarrativeTypistSetup setup;
    private float startTime;
    /// <summary>
    /// Maps text length to string index in <see cref="NarrativeTypistSetup.fullText"/>.
    /// To show N chars of text, take a substring of length richTextLengths[N].
    /// </summary>
    private int[] richTextLengths;
    private RichTextWavy richTextWavy;
    private RichTextShaky richTextShaky;

    // Modified during gameplay.
    private int charsToShow;

    public NarrativeTypistState State { get; set; } = NarrativeTypistState.Uninitialized;

    /// <summary>
    /// Starts the typing process. The process will finish when all
    /// of <paramref name="fullText"/> is displayed.
    /// </summary>
    public virtual void StartTyping(NarrativeTypistSetup narrativeTypistSetup)
    {
        State = NarrativeTypistState.Initializing;
        setup = narrativeTypistSetup;
        (setup.fullText, richTextWavy) = RichTextWavy.ParseTags(setup.fullText);
        (setup.fullText, richTextShaky) = RichTextShaky.ParseTags(setup.fullText);
        richTextLengths = CalculateRichTextLengths(setup.fullText);
        charsToShow = 0;

        speechBubble = GetComponent<SpeechBubbleController>();
        Debug.Assert(speechBubble != null);
        speechBubble.Hide();
        speechBubble.Speaker = setup.speaker;
        speechBubble.Text = "";
        speechBubble.IsPromptVisible = false;
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
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null,
            $"Couldn't find {nameof(AudioSource)} in '{gameObject.GetFullName()}' so can't play speech sounds.");
    }

    protected virtual void FixedUpdate()
    {
        if (State == NarrativeTypistState.Uninitialized) return;
        if (State == NarrativeTypistState.Finished) return;

        if (State == NarrativeTypistState.Initializing)
        {
            UpdateInitializing();
            if (State == NarrativeTypistState.Initializing) return;
        }

        var inputState = inputContext.GetStateAndResetEventFlags();
        HandleInput(inputState);

        UpdateAudiovisuals();

        if (State == NarrativeTypistState.Typing && richTextLengths[charsToShow] == setup.fullText.Length)
            OnTypingFinished();
    }

    #endregion

    /// <summary>
    /// Called when typing has finished but has not yet been acknowledged.
    /// </summary>
    protected virtual void OnTypingFinished()
    {
        State = NarrativeTypistState.UserPrompt;
        charsToShow = richTextLengths.Length - 1;
        // Because UpdateAudioVisuals won't do anything in UserPrompt state, manually update the text.
        speechBubble.Text = setup.fullText;
        speechBubble.IsPromptVisible = true;
    }

    /// <summary>
    /// Called when typing has finished and the player has acknowledged it.
    /// </summary>
    protected virtual void OnTypingAcknowledged()
    {
        State = NarrativeTypistState.Finished;
    }

    private void UpdateInitializing()
    {
        if (!speechBubble.IsInitialized) return;

        State = NarrativeTypistState.Typing;
        startTime = Time.time;

        var textSize = speechBubble.EstimateSize(setup.fullText);
        speechBubble.SetExtent(new Rect(
            speechBubble.transform.localPosition.x,
            speechBubble.transform.localPosition.y,
            textSize.x,
            textSize.y));
        speechBubble.Show();
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
        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: richTextLengths.Length - 1);

        // If nothing has changed, do nothing.
        if (charsToShow == oldCharsToShow
            && !richTextWavy.NeedsUpdate
            && !richTextShaky.NeedsUpdate)
            return;

        // Visual update.
        Debug.Assert(charsToShow < richTextLengths.Length,
            $"Trying to show {charsToShow} non-rich chars from rich text '{setup.fullText}'");
        Debug.Assert(richTextLengths[charsToShow] <= setup.fullText.Length,
            $"Trying to substring {richTextLengths[charsToShow]} from '{setup.fullText}'");
        var currentRichText = setup.fullText.Substring(0, richTextLengths[charsToShow]);
        currentRichText = richTextWavy.RepositionWavyTextChars(currentRichText);
        currentRichText = richTextShaky.RepositionShakyTextChars(currentRichText);
        speechBubble.Text = currentRichText;

        // Audio update.
        if (charsToShow != oldCharsToShow)
        {
            var lastCharIsSpace = speechBubble.Text.Length == 0 ||
                char.IsWhiteSpace(speechBubble.Text[speechBubble.Text.Length - 1]);
            if (!lastCharIsSpace && settings.characterSound != null)
                audioSource?.TryPlay(settings.characterSound);
        }
    }

    /// <summary>
    /// Returns an array that maps text length to length in <paramref name="richText"/>
    /// that will yield that many visible chars. The mapping essentially skips all
    /// TextMeshPro rich text tags.
    /// </summary>
    public static int[] CalculateRichTextLengths(string richText)
    {
        Debug.Assert(richText != null);
        var lengths = new List<int>(richText.Length + 1);

        var state = RichTextParseState.Text;
        for (int i = 0; i < richText.Length; i++)
        {
            switch (state)
            {
                case RichTextParseState.Text:
                    lengths.Add(i);
                    if (richText[i] == '<')
                        state = RichTextParseState.InTag;
                    break;
                case RichTextParseState.InTag:
                    if (richText[i] == '>')
                        state = RichTextParseState.AfterTag;
                    break;
                case RichTextParseState.AfterTag:
                    state = richText[i] == '<'
                        ? RichTextParseState.InTag
                        : RichTextParseState.Text;
                    break;
            }
        }
        lengths.Add(richText.Length);
        return lengths.ToArray();
    }
}
