using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests
{
    public class NarrativeTypistTests
    {
        private void AssertRichTextLengths(IEnumerable<int> expected, string richText) =>
            CollectionAssert.AreEqual(
                expected.ToArray(),
                NarrativeTypist.CalculateRichTextLengths(richText),
                $"Incorrect text lengths for rich text '{richText}'");

        private void AssertWavyText((string, int[]) expected, string richText)
        {
            var actual = NarrativeTypist.ParseWavyTags(richText);
            Assert.AreEqual(expected.Item1, actual.Item1, $"Incorrect output for input '{richText}'");
            CollectionAssert.AreEqual(expected.Item2, actual.Item2, $"Incorrect output for input '{richText}'");
        }

        private void TestWavyCharReposition(string richText, int[] wavyCharIndices)
        {
            var result = NarrativeTypist.RepositionWavyTextChars(richText, wavyCharIndices);
            var richChars = richText.ToCharArray();
            var regex = "<line-height=100%><voffset=0> </voffset><pos=0>" + string.Join("", richChars
                .Select((ch, i) =>
                {
                    var isFirst = i == 0;
                    var isLast = i == richText.Length - 1;
                    var isWavy = wavyCharIndices.Contains(i);
                    var isNextWavy = wavyCharIndices.Contains(i + 1);
                    var isPrevWavy = wavyCharIndices.Contains(i - 1);

                    var chRegex = "";
                    if (isWavy || isFirst || isPrevWavy) chRegex += "<voffset=[-0-9.]+>";
                    chRegex += ch;
                    if (isWavy || isLast || isNextWavy) chRegex += "</voffset>";

                    return chRegex;
                }));
            var isMatch = Regex.IsMatch(result, regex);
            Assert.IsTrue(isMatch, $"Expected match with '{regex}' but was '{result}'");
        }

        [Test]
        public void CalculateRichTextLengths()
        {
            AssertRichTextLengths(Enumerable.Range(0, 14), "Hello, world!");
            AssertRichTextLengths(new[] { 0,1,2, 16,17, 27,28,29 }, @"I <color=""red"">am<#0000FF> ok");
            AssertRichTextLengths(new[] { 0,1,2, 14,15,16, 24,25 }, @"I <size=200%>am </size>ok");
            AssertRichTextLengths(new[] { 0, 4,5, 9,10, 15,16,17, 21 }, @"<b>I <i>am</b> ok</i>");
            AssertRichTextLengths(new[] { 0, 7,8,9,10,11,12, 20 }, @"<b><i>tricky</b></i>");
        }

        [Test]
        public void ParseWavyText()
        {
            AssertWavyText(("foo", new int[0]), "foo");
            AssertWavyText(("foo", new[] { 0, 1, 2 }), "<wavy>foo</wavy>");
            AssertWavyText(("afoo", new[] { 1, 2, 3 }), "a<wavy>foo</wavy>");
            AssertWavyText(("foob", new[] { 0, 1, 2 }), "<wavy>foo</wavy>b");
            AssertWavyText(("foo", new[] { 0, 1, 2 }), "<wavy>foo");
            //Not supported: AssertWavyTags(("f<i>oo", new[] { 0, 4, 5 }), "<wavy>f<i>oo</wavy>");
            AssertWavyText(("foo.bar", new[] { 0, 1, 2, 4, 5, 6 }), "<wavy>foo</wavy>.<wavy>bar</wavy>");
        }

        [Test]
        public void RepositionWavyTextChars()
        {
            TestWavyCharReposition("foo", new int[0]);
            TestWavyCharReposition("foo", new[] { 0, 1, 2 });
            TestWavyCharReposition("foo", new[] { 1, 2 });
            TestWavyCharReposition("foo", new[] { 0, 1 });
            TestWavyCharReposition("afoob", new[] { 1, 3 });
        }
    }
}
