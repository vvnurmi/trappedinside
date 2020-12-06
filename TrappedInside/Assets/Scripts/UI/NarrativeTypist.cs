using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Helper struct to pass parameters to <see cref="NarrativeTypist"/>.
/// </summary>
// Note: This is a class instead of a struct because struct members in classes are not
// displayed properly in the debugger at least with Visual Studio 2017 (15.9.8) and
// Unity 2019.2.18f1. This would complicate debugging NarrativeTypist.
public class NarrativeTypistSetup
{
    public string fullText;
    public string speaker;
    public string[] choices;
}

public struct WavyTextParams
{
    [Tooltip("The maximum displacement of a character, in the object's coordinate system.")]
    public float WaveAmplitude;
    [Tooltip("How many times a character waves back and forth in a second.")]
    public float WaveFrequency;
    [Tooltip("How many characters fit in one wave.")]
    public float WaveLength;

    public override string ToString()
    {
        FormattableString format = $"amplitude={WaveAmplitude} frequency={WaveFrequency} length={WaveLength}";
        return format.ToString(CultureInfo.InvariantCulture);
    }

    public static WavyTextParams Default =>
        new WavyTextParams
        {
            WaveAmplitude = 0.01f,
            WaveFrequency = 3,
            WaveLength = 10,
        };
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
    private static readonly Regex WavyStartTagRegex = new Regex(
        @"<wavy (?:\s+ (amplitude|frequency|length) = ([-0-9.]+) )* >",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    public NarrativeTypistSettings settings;

    /// <summary>
    /// Internal state of <see cref="CalculateRichTextLengths(string)"/>.
    /// </summary>
    private enum RichTextParseState { Text, InTag, AfterTag };

    // Set about once, probably in Start().
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
    /// <summary>
    /// Pair (index, parms) of each char that's supposed to wave, where
    /// index is the char's location in <see cref="NarrativeTypistSetup.fullText"/> and
    /// parms defines the details of the waving.
    /// May be empty.
    /// </summary>
    private (int index, WavyTextParams parms)[] wavyChars;

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
        (setup.fullText, wavyChars) = ParseWavyTags(setup.fullText);
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
        audioSource = GetComponent<AudioSource>();
        Debug.Assert(audioSource != null,
            $"Couldn't find {nameof(AudioSource)} in '{gameObject.GetFullName()}' so can't play speech sounds.");
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
        textComponent.text = setup.fullText;
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

        // If nothing has changed, do nothing.
        if (wavyChars.Length == 0 && charsToShow == oldCharsToShow) return;

        // Visual update.
        Debug.Assert(charsToShow < richTextLengths.Length,
            $"Trying to show {charsToShow} non-rich chars from rich text '{setup.fullText}'");
        Debug.Assert(richTextLengths[charsToShow] <= setup.fullText.Length,
            $"Trying to substring {richTextLengths[charsToShow]} from '{setup.fullText}'");
        var currentRichText = setup.fullText.Substring(0, richTextLengths[charsToShow]);
        currentRichText = RepositionWavyTextChars(currentRichText, wavyChars);
        textComponent.text = currentRichText;

        // Audio update.
        if (charsToShow != oldCharsToShow)
        {
            var lastCharIsSpace = textComponent.text.Length == 0 ||
                char.IsWhiteSpace(textComponent.text[textComponent.text.Length - 1]);
            if (!lastCharIsSpace && settings.characterSound != null)
                audioSource?.TryPlay(settings.characterSound);
        }
    }

    /// <summary>
    /// Parses wavy tags from <paramref name="richText"/>. Returns the pair
    /// (strippedRichText, wavyChars), where
    /// strippedRichText is <paramref name="richText"/> stripped of wavy tags, and
    /// wavyChars is an array of pairs (index, parms) of wavy chars, where
    /// index is the location in strippedRichText of the char, and
    /// parms defines the details of the waving.
    /// </summary>
    public static (string, (int index, WavyTextParams parms)[]) ParseWavyTags(string richText)
    {
        var strippedRichText = new StringBuilder(richText.Length);
        var wavyChars = new List<(int, WavyTextParams)>();
        var index = 0;
        var skippedChars = 0;
        while (index < richText.Length)
        {
            const string EndTag = "</wavy>";
            var startMatch = WavyStartTagRegex.Match(richText, index);
            if (!startMatch.Success) break;

            skippedChars += startMatch.Length;
            var wavyTextStart = startMatch.Index + startMatch.Length;
            var endIndex = richText.IndexOf(EndTag, wavyTextStart);
            var wavyTextEnd = endIndex == -1
                ? richText.Length
                : endIndex;
            var wavyTextLength = wavyTextEnd - wavyTextStart;

            var parms = WavyTextParams.Default;
            Debug.Assert(startMatch.Groups.Count == 3);
            Debug.Assert(startMatch.Groups[1].Captures.Count == startMatch.Groups[2].Captures.Count);

            void ParseAttributeValue(int attributeIndex, out float value)
            {
                var success = float.TryParse(
                    startMatch.Groups[2].Captures[attributeIndex].Value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out value);
                if (!success)
                    Debug.LogWarning($"Failed to parse attribute value in '{startMatch.Value}'");
            }

            for (int attributeIndex = 0; attributeIndex < startMatch.Groups[1].Captures.Count; attributeIndex++)
                switch (startMatch.Groups[1].Captures[attributeIndex].Value)
                {
                    case "amplitude":
                        ParseAttributeValue(attributeIndex, out parms.WaveAmplitude);
                        break;
                    case "frequency":
                        ParseAttributeValue(attributeIndex, out parms.WaveFrequency);
                        break;
                    case "length":
                        ParseAttributeValue(attributeIndex, out parms.WaveLength);
                        break;
                }

            strippedRichText
                .Append(richText, index, startMatch.Index - index)
                .Append(richText, wavyTextStart, wavyTextLength);
            wavyChars.AddRange(Enumerable
                .Range(wavyTextStart - skippedChars, wavyTextLength)
                .Select(i => (i, parms)));
            if (endIndex != -1)
                skippedChars += EndTag.Length;
            index = Math.Min(richText.Length, wavyTextEnd + EndTag.Length);

        }
        strippedRichText.Append(richText, index, richText.Length - index);
        return (strippedRichText.ToString(), wavyChars.ToArray());
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

    /// <summary>
    /// Adds rich text formatting commands to reposition characters that are
    /// in a wavy text interval.
    /// </summary>
    /// <param name="richText">Rich text where to embed wavy char formatting.</param>
    /// <param name="wavyChars">Pair (index, parms) of each char that's supposed to wave, where
    /// index is the char's location in <paramref name="richText"/> and
    /// parms defines the details of the waving.</param>
    public static string RepositionWavyTextChars(string richText, (int index, WavyTextParams parms)[] wavyChars)
    {
        if (wavyChars.Length == 0) return richText;

        var maxAmplitude = wavyChars.Max(x => x.parms.WaveAmplitude);
        var baseVOffset = -maxAmplitude;
        var result = new StringBuilder();

        void AppendWithVOffset(string text, int startIndex, int endIndex, float voffset)
        {
            if (startIndex > text.Length) return;
            if (startIndex == endIndex) return;

            var safeEndIndex = Mathf.Min(endIndex, text.Length);

            // Note: For better performance, don't format the float here but use a precalculated string array.
            result.Append("<voffset=")
                .AppendFormat(CultureInfo.InvariantCulture, "{0:N3}", voffset)
                .Append('>')
                .Append(richText, startIndex, safeEndIndex - startIndex)
                .Append("</voffset>");
        }

        // Note: Without <line-height> the line spacing will change to accommodate <voffset> characters.
        // It makes the whole text block move vertically, which is not wanted.
        result.Append("<line-height=100%><voffset=0> </voffset><pos=0>");

        // Note: Also push the non-offset text down by half the movement delta, and move the waving
        // text down from zero line. If the offset goes positive on the topmost line, it will push the
        // whole text block down, which is not wanted.

        int textProcessedUntilIndex = 0;
        foreach (var (index, parms) in wavyChars)
        {
            Debug.Assert(textProcessedUntilIndex <= index, "Wavy characters are given out of order");
            AppendWithVOffset(richText, textProcessedUntilIndex, index, baseVOffset);
            var voffset = GetWavyCharVerticalOffset(index, maxAmplitude, parms);
            AppendWithVOffset(richText, index, index + 1, voffset);
            textProcessedUntilIndex = index + 1;

            if (textProcessedUntilIndex >= richText.Length) break;
        }
        // The rest of the text.
        AppendWithVOffset(richText, textProcessedUntilIndex, richText.Length, baseVOffset);

        return result.ToString();
    }

    private static float GetWavyCharVerticalOffset(int index, float maxAmplitude, WavyTextParams parms)
    {
        var phase = 2 * Mathf.PI
            * (Time.time * parms.WaveFrequency
                + index / parms.WaveLength);
        return -maxAmplitude - parms.WaveAmplitude * Mathf.Sin(phase);
    }
}
