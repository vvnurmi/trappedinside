using NUnit.Framework;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaSpeechTests : TiaTestBase
    {
        /// <summary>
        /// Creates a speech bubble as a game object that can be given by name to
        /// <see cref="TiaSpeech"/> to clone the actual speech bubble from at run-time.
        /// </summary>
        private void NewSpeechBubble(string speechBubbleName, GameObject tiaRoot)
        {
            var speechBubblePrefab = NewGameObject(speechBubbleName);
            speechBubblePrefab.transform.parent = tiaRoot.transform;
            var textField = speechBubblePrefab.AddComponent<TextMeshProUGUI>();
            textField.tag = "SpeechText";
            {
                var settings = speechBubblePrefab.AddComponent<NarrativeTypistSettings>();
                settings.charsPerSecond = 10;
            }
            speechBubblePrefab.AddComponent<NarrativeTypist>();
        }

        [UnityTest]
        public IEnumerator Speech()
        {
            var richText = "I <size=200%>will</size> say something!";
            var speechBubbleName = "speech bubble";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;
            NewSpeechBubble(speechBubbleName, tiaRoot);

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaSpeech
                {
                    TmpRichText = richText,
                    SpeechBubbleName = speechBubbleName,
                });

            yield return new WaitForSeconds(0.1f);
            var narrativeTypist = testObject.GetComponentInChildren<NarrativeTypist>();
            Debug.Assert(narrativeTypist != null);

            // Type a little and verify that text is appearing.
            yield return new WaitForSeconds(0.4f);
            {
                Assert.AreEqual(NarrativeTypistState.Typing, narrativeTypist.State);
                var tmpUguis = testObject.GetComponentsInChildren<TextMeshProUGUI>();
                Assert.AreEqual(1, tmpUguis.Length, "Not the expected number of TMP texts");
                Assert.AreEqual(richText.Substring(0, 5), tmpUguis[0].text);
            }

            // Type the rest and verify all text appears.
            {
                var settings = testObject.GetComponentInChildren<NarrativeTypistSettings>();
                settings.charsPerSecond = 100;
                yield return new WaitForSeconds(richText.Length / settings.charsPerSecond);
            }
            {
                Assert.AreEqual(NarrativeTypistState.UserPrompt, narrativeTypist.State);
                var tmpUguis = testObject.GetComponentsInChildren<TextMeshProUGUI>();
                Assert.AreEqual(1, tmpUguis.Length, "Not the expected number of TMP texts");
                Assert.AreEqual(richText, tmpUguis[0].text);
            }

            // Acknowledge the user prompt and verify the speech bubble disappears.
            PressSpace();
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
