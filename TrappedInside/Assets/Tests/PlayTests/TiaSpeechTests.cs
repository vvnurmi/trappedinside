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
        /// <see cref="TiaSpeak"/> to clone the actual speech bubble from at run-time.
        /// </summary>
        private void NewSpeechBubble(string speechBubbleName, GameObject tiaRoot, float charsPerSecond)
        {
            var speechBubblePrefab = NewGameObject(speechBubbleName);
            speechBubblePrefab.transform.parent = tiaRoot.transform;

            AddTextFieldComponent(speechBubblePrefab, TiaSpeak.TagText);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeak.TagSpeaker);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeak.TagLeft);
            AddTextFieldComponent(speechBubblePrefab, TiaSpeak.TagRight);

            speechBubblePrefab.AddComponent<AudioSource>();
            speechBubblePrefab.AddComponent<SpeechBubbleController>();
            AddSpriteRendererComponent(speechBubblePrefab, "Sprite 1");
            AddSpriteRendererComponent(speechBubblePrefab, "Sprite 2");
            AddSpriteRendererComponent(speechBubblePrefab, "Sprite 3");
            var settings = ScriptableObject.CreateInstance<NarrativeTypistSettings>();
            settings.charsPerSecond = charsPerSecond;
            var narrativeTypist = speechBubblePrefab.AddComponent<NarrativeTypist>();
            narrativeTypist.settings = settings;
            var narrativeTypistChoice = speechBubblePrefab.AddComponent<NarrativeTypistChoice>();
            narrativeTypistChoice.settings = settings;
        }

        private void AddTextFieldComponent(GameObject speechBubblePrefab, string tag)
        {
            var hostObject = NewGameObject(tag);
            hostObject.transform.parent = speechBubblePrefab.transform;
            var textField = hostObject.AddComponent<TextMeshProUGUI>();
            textField.tag = tag;
        }

        private void AddSpriteRendererComponent(GameObject speechBubblePrefab, string name)
        {
            var spriteObject = NewGameObject(name);
            spriteObject.transform.parent = speechBubblePrefab.transform;
            var sprite = spriteObject.AddComponent<SpriteRenderer>();
            spriteObject.AddComponent<RectTransform>();
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
            AssertFieldContent(expectedText, TiaSpeak.TagText);
            AssertFieldContent(expectedSpeaker, TiaSpeak.TagSpeaker);
            AssertFieldContent(expectedLeftChoice, TiaSpeak.TagLeft);
            AssertFieldContent(expectedRightChoice, TiaSpeak.TagRight);
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
                new TiaSpeak
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
                // Note: With 10 chars per second, in 0.5 seconds, 16 chars will be visible
                // because the rich text tag will appear with the char following it.
                expectedText: richText.Substring(0, 16),
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "",
                expectedRightChoice: "",
                testObject);

            // Type the rest and verify all text appears.
            {
                narrativeTypist.settings.charsPerSecond = 100;
                yield return new WaitForSeconds(richText.Length / narrativeTypist.settings.charsPerSecond);
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
        public IEnumerator SpeechCanBeSkipped()
        {
            var richText = "I <size=200%>will</size> say something!";
            var speechBubbleName = "speech bubble";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;
            NewSpeechBubble(speechBubbleName, tiaRoot, charsPerSecond: 10);

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaSpeak
                {
                    TmpRichText = richText,
                    SpeechBubbleName = speechBubbleName,
                });

            yield return new WaitForSeconds(0.1f);
            var narrativeTypist = testObject.GetComponentInChildren<NarrativeTypist>();
            Assert.IsNotNull(narrativeTypist);

            // Type a little, skip the text and verify that all text has appeared.
            yield return new WaitForSeconds(0.1f);
            PressKey(Key.Space);
            yield return new WaitForSeconds(0.1f);
            Assert.AreEqual(NarrativeTypistState.UserPrompt, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText,
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "",
                expectedRightChoice: "",
                testObject);
        }

        [UnityTest]
        public IEnumerator SpeechWithChoice()
        {
            var richText = "Will this test fail?";
            var leftChoice = "Yes";
            var rightChoice = "No";
            var speechBubbleName = "speech bubble";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;
            NewSpeechBubble(speechBubbleName, tiaRoot, charsPerSecond: 10);

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaSpeak
                {
                    TmpRichText = richText,
                    SpeechBubbleName = speechBubbleName,
                    LeftChoice = leftChoice,
                    RightChoice = rightChoice,
                });

            // Wait for some text to appear. User choices should not be seen yet.
            yield return new WaitForSeconds(1);
            var narrativeTypist = testObject.GetComponentInChildren<NarrativeTypistChoice>();
            Assert.IsNotNull(narrativeTypist);
            Assert.AreEqual(NarrativeTypistState.Typing, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText.Substring(0, 10),
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "",
                expectedRightChoice: "",
                testObject);

            // Wait for the remaining text to appear. User prompts should have appeared.
            yield return new WaitForSeconds(1);
            Assert.AreEqual(NarrativeTypistState.UserPrompt, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText,
                expectedSpeaker: testObject.name,
                expectedLeftChoice: "[" + leftChoice + "]",
                expectedRightChoice: rightChoice,
                testObject);

            // Choose the other choice.
            yield return narrativeTypist.StartCoroutine(PressAndHoldKey(Key.RightArrow));
            Assert.AreEqual(NarrativeTypistState.UserPrompt, narrativeTypist.State);
            AssertTextFields(
                expectedText: richText,
                expectedSpeaker: testObject.name,
                expectedLeftChoice: leftChoice,
                expectedRightChoice: "[" + rightChoice + "]",
                testObject);

            // Acknowledge the user prompt and verify the speech bubble disappears.
            PressKey(Key.Space);
            yield return new WaitForSeconds(0.1f);
            {
                Assert.AreEqual(NarrativeTypistState.Finished, narrativeTypist.State);
                var tmpUguis = testObject.GetComponentsInChildren<TextMeshProUGUI>();
                Assert.AreEqual(0, tmpUguis.Length, $"TMP text was not removed in time:"
                    + string.Join("|", tmpUguis.Select(ugui => ugui.text)));
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
                new TiaSpeak
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
