using NUnit.Framework;
using System.Linq;
using System.Text.RegularExpressions;

namespace Tests
{
    public class RichTextWavyTests
    {
        private void AssertWavyText(
            string expectedStrippedRichText,
            (int index, WavyTextParams parms)[] expectedWavyChars,
            string richText)
        {
            var actual = RichTextWavy.ParseTags(richText);
            Assert.AreEqual(expectedStrippedRichText, actual.Item1, $"Incorrect output for input '{richText}'");
            CollectionAssert.AreEqual(expectedWavyChars, actual.Item2.WavyChars, $"Incorrect output for input '{richText}'");
        }

        private void AssertWavyTextSimple(
            string expectedStrippedRichText,
            int[] expectedWavyCharIndices,
            WavyTextParams expectedWavyCharParms,
            string richText)
        {
            var expectedWavyChars = expectedWavyCharIndices
                .Select(i => (i, expectedWavyCharParms))
                .ToArray();
            AssertWavyText(expectedStrippedRichText, expectedWavyChars, richText);
        }

        private void TestWavyCharReposition(string richText, int[] wavyCharIndices)
        {
            var wavyChars = wavyCharIndices
                .Select(i => (i, WavyTextParams.Default))
                .ToArray();
            var richTextWavy = new RichTextWavy(wavyChars);
            var result = richTextWavy.RepositionWavyTextChars(richText);
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
        public void ParseWavyText()
        {
            var parms = WavyTextParams.Default;
            AssertWavyTextSimple("foo", new int[0], parms, "foo");
            AssertWavyTextSimple("foo", new[] { 0, 1, 2 }, parms, "<wavy>foo</wavy>");
            AssertWavyTextSimple("afoo", new[] { 1, 2, 3 }, parms, "a<wavy>foo</wavy>");
            AssertWavyTextSimple("foob", new[] { 0, 1, 2 }, parms, "<wavy>foo</wavy>b");
            AssertWavyTextSimple("foo", new[] { 0, 1, 2 }, parms, "<wavy>foo");
            //Not supported: AssertWavyTextSimple(("f<i>oo", new[] { 0, 4, 5 }, parms, "<wavy>f<i>oo</wavy>");
            AssertWavyTextSimple("foo.bar", new[] { 0, 1, 2, 4, 5, 6 }, parms, "<wavy>foo</wavy>.<wavy>bar</wavy>");
        }

        [Test]
        public void ParseWavyTextWithAttributes()
        {
            var parms1 = new WavyTextParams { WaveAmplitude = 2.2f, WaveFrequency = 3.3f, WaveLength = 4.4f };
            var parms2 = new WavyTextParams { WaveAmplitude = 1, WaveFrequency = 2, WaveLength = 3 };
            var parmsA = WavyTextParams.Default; parmsA.WaveAmplitude = 5;
            var parmsF = WavyTextParams.Default; parmsF.WaveFrequency = 6;
            var parmsL = WavyTextParams.Default; parmsL.WaveLength = 7;
            AssertWavyTextSimple(
                "foo",
                new[] { 0, 1, 2 },
                parms1,
                "<wavy amplitude=2.2 frequency=3.3 length=4.4>foo</wavy>");
            AssertWavyText(
                ".a.b.",
                new[] { (1, parms1), (3, parms2) },
                ".<wavy amplitude=2.2 frequency=3.3 length=4.4>a</wavy>"
                + ".<wavy amplitude=1 frequency=2 length=3>b</wavy>.");
            AssertWavyTextSimple("foo", new[] { 0, 1, 2 }, parmsA, "<wavy amplitude=5>foo</wavy>");
            AssertWavyTextSimple("foo", new[] { 0, 1, 2 }, parmsF, "<wavy frequency=6>foo</wavy>");
            AssertWavyTextSimple("foo", new[] { 0, 1, 2 }, parmsL, "<wavy length=7>foo</wavy>");
        }

        [Test]
        public void RepositionWavyTextChars()
        {
            Assert.AreEqual("foo", new RichTextWavy(new (int, WavyTextParams)[0]).RepositionWavyTextChars("foo"));
            TestWavyCharReposition("foo", new[] { 0, 1, 2 });
            TestWavyCharReposition("foo", new[] { 1, 2 });
            TestWavyCharReposition("foo", new[] { 0, 1 });
            TestWavyCharReposition("afoob", new[] { 1, 3 });
        }
    }
}
