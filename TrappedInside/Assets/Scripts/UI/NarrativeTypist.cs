﻿using System.Collections.Generic;
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
    /// <summary>
    /// Internal state of <see cref="CalculateRichTextLengths(string)"/>.
    /// </summary>
    private enum RichTextParseState { Text, InTag, AfterTag };

    // Set about once, probably in Start().
    protected NarrativeTypistSettings settings;
    protected AudioSource audioSource;
    protected ITIInputContext inputContext;
    private TMPro.TextMeshProUGUI textComponent;
    private NarrativeTypistSetup setup;
    private float startTime;
    /// <summary>
    /// Maps text length to string index in <see cref="NarrativeTypistSetup.fullText"/>.
    /// To show N chars of text, take a substring of length richTextLengths[N].
    /// </summary>
    private int[] richTextLengths;

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
        richTextLengths = CalculateRichTextLengths(setup.fullText);
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
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null,
            $"Couldn't find {nameof(AudioSource)} in {nameof(NarrativeTypist)}, so can't play speech sounds.");
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
        charsToShow = richTextLengths[richTextLengths.Length - 1];
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
        var oldCharsToShow = charsToShow;
        charsToShow = Mathf.Clamp(
            value: Mathf.RoundToInt((Time.time - startTime) * settings.charsPerSecond),
            min: charsToShow,
            max: richTextLengths.Length - 1);
        if (charsToShow == oldCharsToShow) return;

        // Visual update.
        Debug.Assert(charsToShow < richTextLengths.Length,
            $"Trying to show {charsToShow} non-rich chars from rich text '{setup.fullText}'");
        Debug.Assert(richTextLengths[charsToShow] <= setup.fullText.Length,
            $"Trying to substring {richTextLengths[charsToShow]} from '{setup.fullText}'");
        textComponent.text = setup.fullText.Substring(0, richTextLengths[charsToShow]);

        // Audio update.
        var lastCharIsSpace = textComponent.text.Length == 0 ||
            char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
        if (!lastCharIsSpace)
            audioSource?.TryPlay(settings.characterSound);
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
