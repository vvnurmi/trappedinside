using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    public class NarrativeTypistTests
    {
        private void AssertRichTextLengths(IEnumerable<int> expected, string richText) =>
            CollectionAssert.AreEqual(
                expected.ToArray(),
                NarrativeTypist.CalculateRichTextLengths(richText),
                $"Incorrect text lengths for rich text '{richText}'");

        [Test]
        public void CalculateRichTextLengths()
        {
            AssertRichTextLengths(Enumerable.Range(0, 14), "Hello, world!");
            AssertRichTextLengths(new[] { 0,1,2, 16,17, 27,28,29 }, @"I <color=""red"">am<#0000FF> ok");
            AssertRichTextLengths(new[] { 0,1,2, 14,15,16, 24,25 }, @"I <size=200%>am </size>ok");
            AssertRichTextLengths(new[] { 0, 4,5, 9,10, 15,16,17, 21 }, @"<b>I <i>am</b> ok</i>");
            AssertRichTextLengths(new[] { 0, 7,8,9,10,11,12, 20 }, @"<b><i>tricky</b></i>");
        }
    }
}
