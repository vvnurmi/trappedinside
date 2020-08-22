using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaSpeechTests : TiaTestBase
    {
        /// <summary>
        /// Creates a speech bubble as a game object that can be given by name to
        /// <see cref="TiaSpeech"/> to clone the actual speech bubble from at run-time.
        /// </summary>
        private void NewSpeechBubble(string speechBubbleName, GameObject tiaRoot, float charsPerSecond)
        {
            var speechBubblePrefab = NewGameObject(speechBubbleName);
            speechBubblePrefab.transform.parent = tiaRoot.transform;

            AddTextFieldComponent(speechBubblePrefab, TiaSpeech.TagText);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeech.TagSpeaker);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeech.TagLeft);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeech.TagRight);

            var settings = speechBubblePrefab.AddComponent<NarrativeTypistSettings>();
            settings.charsPerSecond = charsPerSecond;

            speechBubblePrefab.AddComponent<NarrativeTypist>();
        }

        private void AddTextFieldComponent(GameObject speechBubblePrefab, string tag)
        {
            var hostObject = NewGameObject(tag);
            hostObject.transform.parent = speechBubblePrefab.transform;
            var textField = hostObject.AddComponent<TextMeshProUGUI>();
            textField.tag = tag;
        }

        /// <summary>
        /// Asserts that <paramref name="testObject"/> has the expected text fields
        /// and that their contents match the expected strings.
        /// </summary>
        private void AssertTextFields(
            string expectedText,
            string expectedSpeaker,
            string expectedLeftChoice,
            string expectedRightChoice,
            GameObject testObject)
        {
            var tmpUguis = testObject.GetComponentsInChildren<TextMeshProUGUI>();
            Assert.AreEqual(4, tmpUguis.Length, "Wrong number of text fields in speech bubble");
            void AssertFieldContent(string expectedContent, string tag)
            {
                var taggedUgui = tmpUguis.First(ugui => ugui.CompareTag(tag));
                Assert.IsNotNull(taggedUgui, $"Text field with tag '{tag}' not found in speech bubble");
                Assert.AreEqual(expectedContent, taggedUgui.text);
            }
            AssertFieldContent(expectedText, TiaSpeech.TagText);
            AssertFieldContent(expectedSpeaker, TiaSpeech.TagSpeaker);
            AssertFieldContent(expectedLeftChoice, TiaSpeech.TagLeft);
            AssertFieldContent(expectedRightChoice, TiaSpeech.TagRight);
        }

        [UnityTest]
        public IEnumerator Speech()
        {
            var richText = "I <size=200%>will</size> say something!";
            var speechBubbleName = "speech bubble";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;
            NewSpeechBubble(speechBubbleName, tiaRoot, charsPerSecond: 10);

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaSpeech
                {
                    TmpRichText = richText,
                    SpeechBubbleName = speechBubbleName,
                });

            yield return new WaitForSeconds(0.1f);
            var narrativeTypist = testObject.GetComponentInChildren<NarrativeTypist>();
            Assert.IsNotNull(narrativeTypist);

            // Type a little and verify that text is appearing.
            yield return new WaitForSeconds(0.4f);
            Assert.AreEqual(NarrativeTypistState.Typing, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText.Substring(0, 5),
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "",
                expectedRightChoice: "",
                testObject);

            // Type the rest and verify all text appears.
            {
                var settings = testObject.GetComponentInChildren<NarrativeTypistSettings>();
                settings.charsPerSecond = 100;
                yield return new WaitForSeconds(richText.Length / settings.charsPerSecond);
            }
            Assert.AreEqual(NarrativeTypistState.UserPrompt, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText,
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "",
                expectedRightChoice: "",
                testObject);

            // Acknowledge the user prompt and verify the speech bubble disappears.
            PressKey(Key.Space);
            yield return new WaitForSeconds(0.1f);
            {
                Assert.AreEqual(NarrativeTypistState.Finished, narrativeTypist.State);
                var tmpUguis = testObject.GetComponentsInChildren<TextMeshProUGUI>();
                Assert.AreEqual(0, tmpUguis.Length, $"Text fields were not removed in time:"
                    + string.Join(", ", tmpUguis.Select(ugui => ugui.tag ?? ugui.text ?? "???")));
            }
        }

        [UnityTest]
        public IEnumerator SpeechThrowsIfSpeechBubbleIsNotFound()
        {
            var richText = "I <size=200%>will</size> say something!";
            var speechBubbleName = "nonexistent";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaSpeech
                {
                    TmpRichText = richText,
                    SpeechBubbleName = speechBubbleName,
                },
                new TiaDeactivate());

            LogAssert.Expect(LogType.Assert, new Regex($"couldn't find speech bubble.*{speechBubbleName}"));
            yield return new WaitForSeconds(0.1f);
        }
    }
}
