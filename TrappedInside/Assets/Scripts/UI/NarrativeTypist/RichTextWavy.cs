using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class RichTextWavy
{
    private static readonly Regex WavyStartTagRegex = new Regex(
        @"<wavy (?:\s+ (amplitude|frequency|length) = ([-0-9.]+) )* >",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    /// <summary>
    /// Call <see cref="ParseWavyTags(string)"/> to create a <see cref="RichTextWavy"/> instance.
    /// </summary>
    private RichTextWavy() { }

    /// <summary>
    /// Internals exposed for automated tests.
    /// </summary>
    internal RichTextWavy((int index, WavyTextParams parms)[] wavyChars)
    {
        WavyChars = wavyChars;
    }

    /// <summary>
    /// If true, then the visual representation of the text needs to be updated this/every frame.
    /// </summary>
    public bool NeedsUpdate => WavyChars.Length > 0;

    /// <summary>
    /// Pair (index, parms) of each char that's supposed to wave, where
    /// index is the char's location in <see cref="NarrativeTypistSetup.fullText"/> and
    /// parms defines the details of the waving.
    /// May be empty.
    /// Internal access for automated tests.
    /// </summary>
    internal (int index, WavyTextParams parms)[] WavyChars;

    /// <summary>
    /// Parses wavy tags from <paramref name="richText"/>.
    /// Returns <paramref name="richText"/> stripped of wavy tags
    /// and a <see cref="RichTextWavy"/> instance that implements the waviness.
    /// </summary>
    public static (string, RichTextWavy) ParseWavyTags(string richText)
    {
        var strippedRichText = new StringBuilder(richText.Length);
        var richTextWavy = new RichTextWavy();
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
        richTextWavy.WavyChars = wavyChars.ToArray();
        return (strippedRichText.ToString(), richTextWavy);
    }

    /// <summary>
    /// Adds rich text formatting commands to reposition characters that are
    /// in a wavy text interval.
    /// </summary>
    /// <param name="richText">Rich text where to embed wavy char formatting.</param>
    public string RepositionWavyTextChars(string richText)
    {
        if (WavyChars.Length == 0) return richText;

        var maxAmplitude = WavyChars.Max(x => x.parms.WaveAmplitude);
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
        foreach (var (index, parms) in WavyChars)
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
