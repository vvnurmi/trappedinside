using NUnit.Framework;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests
{
    public class RichTextShakyTests
    {
        private void AssertShakyText(
            string expectedStrippedRichText,
            ShakyCharInterval[] expectedShakyChars,
            string richText)
        {
            var actual = RichTextShaky.ParseTags(richText);
            Assert.AreEqual(expectedStrippedRichText, actual.Item1, $"Incorrect output for input '{richText}'");
            CollectionAssert.AreEqual(expectedShakyChars, actual.Item2.ShakyChars, $"Incorrect output for input '{richText}'");
        }

        private void AssertShakyTextSimple(
            string expectedStrippedRichText,
            (int start, int end)[] expectedShakyCharIntervals,
            ShakyTextParams expectedShakyCharParms,
            string richText)
        {
            var expectedShakyChars = expectedShakyCharIntervals
                .Select(i => new ShakyCharInterval
                {
                    startIndex = i.start,
                    endIndex = i.end,
                    parms = expectedShakyCharParms,
                })
                .ToArray();
            AssertShakyText(expectedStrippedRichText, expectedShakyChars, richText);
        }
        /*
        private void TestShakyCharReposition(string richText, int[] shakyCharIndices)
        {
            var shakyChars = shakyCharIndices
                .Select(i => (i, ShakyTextParams.Default))
                .ToArray();
            var richTextShaky = new RichTextShaky(shakyChars);
            var result = richTextShaky.RepositionShakyTextChars(richText);
            var richChars = richText.ToCharArray();
            var regex = "<line-height=100%><voffset=0> </voffset><pos=0>" + string.Join("", richChars
                .Select((ch, i) =>
                {
                    var isFirst = i == 0;
                    var isLast = i == richText.Length - 1;
                    var isShaky = shakyCharIndices.Contains(i);
                    var isNextWavy = shakyCharIndices.Contains(i + 1);
                    var isPrevWavy = shakyCharIndices.Contains(i - 1);

                    var chRegex = "";
                    if (isShaky || isFirst || isPrevWavy) chRegex += "<voffset=[-0-9.]+>";
                    chRegex += ch;
                    if (isShaky || isLast || isNextWavy) chRegex += "</voffset>";

                    return chRegex;
                }));
            var isMatch = Regex.IsMatch(result, regex);
            Assert.IsTrue(isMatch, $"Expected match with '{regex}' but was '{result}'");
        }
        */
        [Test]
        public void ParseWavyText()
        {
            var parms = ShakyTextParams.Default;
            AssertShakyTextSimple("foo", new (int, int)[0], parms, "foo");
            // Triggers Debug.Assert which apparently can't be unit tested:
            //AssertShakyTextSimple("foo", new[] { (0, 3) }, parms, "<shaky>foo</shaky>");
            AssertShakyTextSimple("afoob", new[] { (1, 4) }, parms, "a<shaky>foo</shaky>b");
            //Not supported: AssertShakyTextSimple(("f<i>oo", new[] { (0, 1), (4, 6) }, parms, "<shaky>f<i>oo</shaky>");
            AssertShakyTextSimple("afoo..barb", new[] { (1, 4), (6, 9) }, parms, "a<shaky>foo</shaky>..<shaky>bar</shaky>b");
        }
        /*
        [Test]
        public void RepositionWavyTextChars()
        {
            Assert.AreEqual("foo", new RichTextWavy(new (int, WavyTextParams)[0]).RepositionWavyTextChars("foo"));
            TestShakyCharReposition("foo", new[] { 0, 1, 2 });
            TestShakyCharReposition("foo", new[] { 1, 2 });
            TestShakyCharReposition("foo", new[] { 0, 1 });
            TestShakyCharReposition("afoob", new[] { 1, 3 });
        }*/
    }
}
