using NUnit.Framework;
using System.Linq;
using System.Text;
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

        private void TestShakyCharReposition(string richText, (int, int)[] shakyCharIntervals)
        {
            var shakyCharIndices = shakyCharIntervals
                .SelectMany(interval => Enumerable.Range(interval.Item1, interval.Item2 - interval.Item1))
                .ToLookup(i => i);
            var shakyChars = shakyCharIntervals
                .Select(i => new ShakyCharInterval
                {
                    startIndex = i.Item1,
                    endIndex = i.Item2,
                    parms = ShakyTextParams.Default,
                })
                .ToArray();
            var richTextShaky = new RichTextShaky(shakyChars);
            var result = richTextShaky.RepositionShakyTextChars(richText);
            var richChars = richText.ToCharArray();
            var regex = string.Join("", richChars
                .Select((ch, i) =>
                {
                    var isShaky = shakyCharIndices.Contains(i);
                    var isNextShaky = shakyCharIndices.Contains(i + 1);
                    var isPrevShaky = shakyCharIndices.Contains(i - 1);
                    var pattern = new StringBuilder();
                    if (isPrevShaky && !isShaky)
                        pattern.Append("<cspace=0>");
                    if (isNextShaky || isShaky)
                        pattern.Append("<cspace=[-0-9.]+>");
                    pattern.Append(ch);
                    return pattern.ToString();
                }));
            var isMatch = Regex.IsMatch(result, regex);
            Assert.IsTrue(isMatch, $"Expected match with '{regex}' but was '{result}'");
        }

        [Test]
        public void ParseShakyText()
        {
            var parms = ShakyTextParams.Default;
            AssertShakyTextSimple("foo", new (int, int)[0], parms, "foo");
            // Triggers Debug.Assert which apparently can't be unit tested:
            //AssertShakyTextSimple("foo", new[] { (0, 3) }, parms, "<shaky>foo</shaky>");
            AssertShakyTextSimple("afoob", new[] { (1, 4) }, parms, "a<shaky>foo</shaky>b");
            //Not supported: AssertShakyTextSimple(("f<i>oo", new[] { (0, 1), (4, 6) }, parms, "<shaky>f<i>oo</shaky>");
            AssertShakyTextSimple("afoo.barb", new[] { (1, 4), (5, 8) }, parms, "a<shaky>foo</shaky>.<shaky>bar</shaky>b");
        }

        [Test]
        public void ParseShakyTextWithAttributes()
        {
            var parms1 = new ShakyTextParams { Amplitude = 0.02f };
            var parms2 = new ShakyTextParams { Amplitude = 5f };
            AssertShakyTextSimple("foo", new (int, int)[0], parms1, "foo");
            AssertShakyTextSimple("afoob", new[] { (1, 4) }, parms1, "a<shaky amplitude=0.02>foo</shaky>b");
            AssertShakyText(
                "afoo.barb",
                new[]
                {
                    new ShakyCharInterval { startIndex = 1, endIndex = 4, parms = parms1 },
                    new ShakyCharInterval { startIndex = 5, endIndex = 8, parms = parms2 },
                },
                "a<shaky amplitude=0.02>foo</shaky>.<shaky amplitude=5>bar</shaky>b");
        }

        [Test]
        public void RepositionShakyTextChars()
        {
            Assert.AreEqual("foo", new RichTextShaky(new ShakyCharInterval[0]).RepositionShakyTextChars("foo"));
            TestShakyCharReposition("afoob", new[] { (1, 4) });
            TestShakyCharReposition("afoo.barb", new[] { (1, 4), (5, 8) });
        }
    }
}
