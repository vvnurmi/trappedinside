using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// Defines an interval of chars that's supposed to shake.
/// <see cref="startIndex"/> and <see cref="endIndex"/> are the indices of the
/// first char and one after the last char, in the related rich text
/// <see cref="NarrativeTypistSetup.fullText"/>.
/// <see cref="parms"/> defines the details of the shaking.
/// </summary>
internal struct ShakyCharInterval
{
    public int startIndex;
    public int endIndex;
    public ShakyTextParams parms;

    public override string ToString() =>
        $"({startIndex}, {endIndex}, {parms})";
}

/// <summary>
/// Manages shaky text in a TextMeshPro rich text string.
/// </summary>
public class RichTextShaky
{
    private const int CSpaceDigits = 3;
    private static readonly string CSpaceFormatString = "{0:N" + CSpaceDigits + "}";
    private static readonly Regex ShakyStartTagRegex = new Regex(
        @"<shaky (?:\s+ (amplitude) = ([-0-9.]+) )* >",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

    /// <summary>
    /// Call <see cref="ParseTags(string)"/> to create a <see cref="RichTextShaky"/> instance.
    /// </summary>
    private RichTextShaky() { }

    /// <summary>
    /// Internals exposed for automated tests.
    /// </summary>
    internal RichTextShaky(ShakyCharInterval[] shakyChars)
    {
        ShakyChars = shakyChars;
    }

    /// <summary>
    /// If true, then the visual representation of the text needs to be updated this/every frame.
    /// </summary>
    public bool NeedsUpdate => ShakyChars.Length > 0;

    /// <summary>
    /// Intervals of shaky chars in <see cref="NarrativeTypistSetup.fullText"/>.
    /// May be empty.
    /// Internal access for automated tests.
    /// </summary>
    internal ShakyCharInterval[] ShakyChars;

    /// <summary>
    /// Parses shaky tags from <paramref name="richText"/>.
    /// Returns <paramref name="richText"/> stripped of shaky tags
    /// and a <see cref="RichTextShaky"/> instance that implements the shakiness.
    /// </summary>
    public static (string, RichTextShaky) ParseTags(string richText)
    {
        var strippedRichText = new StringBuilder(richText.Length);
        var richTextShaky = new RichTextShaky();
        var shakyChars = new List<ShakyCharInterval>();
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
                    case "amplitude":
                        ParseAttributeValue(attributeIndex, out parms.Amplitude);
                        break;
                }

            strippedRichText
                .Append(richText, index, startMatch.Index - index)
                .Append(richText, shakyTextStart, shakyTextLength);
            shakyChars.Add(new ShakyCharInterval {
                startIndex = shakyTextStart - skippedChars,
                endIndex = shakyTextEnd - skippedChars,
                parms = parms,
            });
            if (endIndex != -1)
                skippedChars += EndTag.Length;
            index = Math.Min(richText.Length, shakyTextEnd + EndTag.Length);

        }
        strippedRichText.Append(richText, index, richText.Length - index);
        if (shakyChars.Count > 0)
        {
            Debug.Assert(shakyChars.First().startIndex > 0,
                "Unsupported text markup: There needs to be one non-shaky char before the first shaky text interval");
            Debug.Assert(shakyChars.Last().endIndex < richText.Length - 1,
                "Unsupported text markup: There needs to be one non-shaky char after the last shaky text interval");
            for (int i = 0; i < shakyChars.Count - 1; i++)
            {
                Debug.Assert(shakyChars[i].startIndex <= shakyChars[i].endIndex,
                    "Shaky text internal error: Interval is backwards");
                Debug.Assert(shakyChars[i].endIndex < shakyChars[i + 1].startIndex,
                    "Unsupported text markup: There needs to be at least one non-shaky char between consecutive shaky text intervals");
            }
        }
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
                .AppendFormat(CultureInfo.InvariantCulture, CSpaceFormatString, cspace)
                .Append('>');
            AppendSafelyNoChecks(text, startIndex, endIndex);
        }

        // Note: Also push the non-offset text down by half the movement delta, and move the waving
        // text down from zero line. If the offset goes positive on the topmost line, it will push the
        // whole text block down, which is not wanted.

        int textProcessedUntilIndex = 0;
        foreach (var interval in ShakyChars)
        {
            Debug.Assert(textProcessedUntilIndex <= interval.startIndex, "Shaky characters are given out of order");
            AppendSafely(richText, textProcessedUntilIndex, interval.startIndex - 1);

            var previousOffset = 0f;
            for (int index = interval.startIndex - 1; index < interval.endIndex - 1; index++)
            {
                var (cspace, offset) = GetShakyCharCharacterSpacing(previousOffset, interval.parms);
                previousOffset = offset;
                AppendWithCSpace(richText, index, index + 1, cspace);
                textProcessedUntilIndex = index + 1;
                if (textProcessedUntilIndex >= richText.Length) break;
            }

            if (textProcessedUntilIndex >= richText.Length) break;
            AppendWithCSpace(richText, interval.endIndex - 1, interval.endIndex, -previousOffset);
            textProcessedUntilIndex++;
            // Note: <cspace=0> is like </cspace> except that the latter would
            // need to be placed one char further to have the exact same effect.
            result.Append("<cspace=0>");
        }
        // The rest of the text.
        AppendSafely(richText, textProcessedUntilIndex, richText.Length);

        return result.ToString();
    }

    private static (float cspace, float offset) GetShakyCharCharacterSpacing(
        float previousOffset,
        ShakyTextParams parms)
    {
        var charOffset = parms.Amplitude * (1 - 2 * RandomNumber.NextDouble());
        // Round the offset to the precision that's used in the rich text attribute.
        var roundedCharOffset = (float)Math.Round(charOffset, CSpaceDigits);
        return (roundedCharOffset - previousOffset, roundedCharOffset);
    }
}
