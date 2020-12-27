using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Manages shaky text in a TextMeshPro rich text string.
/// </summary>
public class RichTextShaky
{
    private static readonly Regex ShakyStartTagRegex = new Regex(
        @"<shaky (?:\s+ (x|y|z) = ([-0-9.]+) )* >",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    /// <summary>
    /// Call <see cref="ParseTags(string)"/> to create a <see cref="RichTextShaky"/> instance.
    /// </summary>
    private RichTextShaky() { }

    /// <summary>
    /// Internals exposed for automated tests.
    /// </summary>
    internal RichTextShaky((int index, ShakyTextParams parms)[] shakyChars)
    {
        ShakyChars = shakyChars;
    }

    /// <summary>
    /// If true, then the visual representation of the text needs to be updated this/every frame.
    /// </summary>
    public bool NeedsUpdate => ShakyChars.Length > 0;

    /// <summary>
    /// Pair (index, parms) of each char that's supposed to shake, where
    /// index is the char's location in <see cref="NarrativeTypistSetup.fullText"/> and
    /// parms defines the details of the shaking.
    /// May be empty.
    /// Internal access for automated tests.
    /// </summary>
    // TODO !!! store intervals, because first and last chars need special treatment
    internal (int index, ShakyTextParams parms)[] ShakyChars;

    /// <summary>
    /// Parses shaky tags from <paramref name="richText"/>.
    /// Returns <paramref name="richText"/> stripped of shaky tags
    /// and a <see cref="RichTextShaky"/> instance that implements the shakiness.
    /// </summary>
    public static (string, RichTextShaky) ParseTags(string richText)
    {
        var strippedRichText = new StringBuilder(richText.Length);
        var richTextShaky = new RichTextShaky();
        var shakyChars = new List<(int, ShakyTextParams)>();
        var index = 0;
        var skippedChars = 0;
        while (index < richText.Length)
        {
            const string EndTag = "</shaky>";
            var startMatch = ShakyStartTagRegex.Match(richText, index);
            if (!startMatch.Success) break;

            skippedChars += startMatch.Length;
            var shakyTextStart = startMatch.Index + startMatch.Length;
            var endIndex = richText.IndexOf(EndTag, shakyTextStart);
            var shakyTextEnd = endIndex == -1
                ? richText.Length
                : endIndex;
            var shakyTextLength = shakyTextEnd - shakyTextStart;

            var parms = ShakyTextParams.Default;
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
                    case "x":
                        ParseAttributeValue(attributeIndex, out parms.ShakeFoo);
                        break;
                }

            strippedRichText
                .Append(richText, index, startMatch.Index - index)
                .Append(richText, shakyTextStart, shakyTextLength);
            shakyChars.AddRange(Enumerable
                .Range(shakyTextStart - skippedChars, shakyTextLength)
                .Select(i => (i, parms)));
            if (endIndex != -1)
                skippedChars += EndTag.Length;
            index = Math.Min(richText.Length, shakyTextEnd + EndTag.Length);

        }
        strippedRichText.Append(richText, index, richText.Length - index);
        richTextShaky.ShakyChars = shakyChars.ToArray();
        return (strippedRichText.ToString(), richTextShaky);
    }

    /// <summary>
    /// Adds rich text formatting commands to reposition characters that are
    /// in a shaky text interval.
    /// </summary>
    /// <param name="richText">Rich text where to embed shaky char formatting.</param>
    public string RepositionShakyTextChars(string richText)
    {
        if (ShakyChars.Length == 0) return richText;

        var result = new StringBuilder();

        void AppendSafely(string text, int startIndex, int endIndex)
        {
            if (startIndex > text.Length) return;
            if (startIndex == endIndex) return;

            AppendSafelyNoChecks(text, startIndex, endIndex);
        }

        void AppendSafelyNoChecks(string text, int startIndex, int endIndex)
        {
            var safeEndIndex = Mathf.Min(endIndex, text.Length);
            result.Append(text, startIndex, safeEndIndex - startIndex);
        }

        void AppendWithCSpace(string text, int startIndex, int endIndex, float cspace)
        {
            if (startIndex > text.Length) return;
            if (startIndex == endIndex) return;

            // Note: For better performance, don't format the float here but use a precalculated string array.
            result.Append("<cspace=")
                .AppendFormat(CultureInfo.InvariantCulture, "{0:N3}", cspace)
                .Append('>');
            AppendSafelyNoChecks(text, startIndex, endIndex);
        }

        // Note: Without <line-height> the line spacing will change to accommodate <voffset> characters.
        // It makes the whole text block move vertically, which is not wanted.
        result.Append("<line-height=100%><voffset=0> </voffset><pos=0>");

        // Note: Also push the non-offset text down by half the movement delta, and move the waving
        // text down from zero line. If the offset goes positive on the topmost line, it will push the
        // whole text block down, which is not wanted.

        int textProcessedUntilIndex = 0;
        var previousOffset = 0f;
        var lastCspacedIndex = 0;
        foreach (var (index, parms) in ShakyChars)
        {
            Debug.Assert(textProcessedUntilIndex <= index, "Shaky characters are given out of order");
            AppendSafely(richText, textProcessedUntilIndex, index);
            // TODO !!! <cspace={-previousOffset}> for the first char after a contiguous shaky run
            if (index != lastCspacedIndex + 1)
                previousOffset = 0;
            lastCspacedIndex = index;
            var (cspace, offset) = GetShakyCharCharacterSpacing(previousOffset, parms);
            previousOffset = offset;
            AppendWithCSpace(richText, index, index + 1, cspace);
            textProcessedUntilIndex = index + 1;

            if (textProcessedUntilIndex >= richText.Length) break;
        }
        // The rest of the text.
        // TODO !!! <cspace={-previousOffset}> for the first remaining char
        AppendSafely(richText, textProcessedUntilIndex, richText.Length);

        return result.ToString();
    }

    private static (float cspace, float offset) GetShakyCharCharacterSpacing(
        float previousOffset,
        ShakyTextParams parms)
    {
        var amplitude = 0.005f;
        var charOffset = amplitude * (1 - 2 * (float)RandomNumber.NextDouble());
        return (charOffset - previousOffset, charOffset);
    }
}
