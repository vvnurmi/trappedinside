using NUnit.Framework;
using System;
using System.Globalization;

namespace Tests
{
    public class TiaSerializationTests
    {
        [Test]
        public void BasicProperties()
        {
            var scriptName = "Test Script";
            var playOnStart = true;

            var serialized = $@"
---
Name: {scriptName}
AutoPlay: {playOnStart}";

            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(scriptName, tiaScript.ScriptName);
            Assert.AreEqual(playOnStart, tiaScript.PlayOnStart);
        }

        [Test]
        public void ActivationStep()
        {
            AssertActionStep<TiaActivate>("!Activate", step => { });
            AssertActionStep<TiaDeactivate>("!Deactivate", step => { });
        }

        [Test]
        public void AnimationStep()
        {
            var animationName = "Test Animation";
            var deserializedAction = $@"
!Animation
Name: {animationName}";

            void AssertProperties(TiaAnimation step)
            {
                Assert.AreEqual(animationName, step.AnimationName);
            }
            AssertActionStep<TiaAnimation>(deserializedAction, AssertProperties);
        }

        [Test]
        public void MoveStep()
        {
            var CloseEnough = 1e-6;
            var durationSeconds = 2.5f;
            var curveName = "Test Curve";
            var deserializedAction = $@"
!Move
Curve: {curveName}
Seconds: {durationSeconds.ToString(CultureInfo.InvariantCulture)}";

            void AssertProperties(TiaMove step)
            {
                Assert.AreEqual(curveName, step.CurveName);
                Assert.AreEqual(durationSeconds, step.DurationSeconds, CloseEnough);
            }
            AssertActionStep<TiaMove>(deserializedAction, AssertProperties);
        }

        [Test]
        public void PauseStep()
        {
            var pauseSeconds = 2.5f;
            var deserializedAction = $@"
!Pause
Seconds: {pauseSeconds.ToString(CultureInfo.InvariantCulture)}";

            void AssertProperties(TiaPause step)
            {
                Assert.AreEqual(pauseSeconds, step.DurationSeconds);
            }
            AssertActionStep<TiaPause>(deserializedAction, AssertProperties);
        }

        [Test]
        public void PlayScriptStep()
        {
            var scriptName = "Script To Play";
            var deserializedAction = $@"
!PlayScript
Name: {scriptName}";
            void AssertProperties(TiaPlayScript step)
            {
                Assert.AreEqual(scriptName, step.ScriptName);
            }
            AssertActionStep<TiaPlayScript>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeechStep()
        {
            var richText = "I <size=200%>will</size> say something!";
            var deserializedAction = $@"
!Speech
Text: {richText}";
            void AssertProperties(TiaSpeech step)
            {
                Assert.AreEqual(richText, step.TmpRichText);
                Assert.AreEqual(1, step.TypingSpeedMultiplier);
                Assert.IsTrue(step.IsModal);
            }
            AssertActionStep<TiaSpeech>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeechStepWithSpeed()
        {
            var richText = "I <size=200%>will</size> say something!";
            var typingSpeedMultiplier = 1.2f;
            var deserializedAction = $@"
!Speech
Text: {richText}
Speed: {typingSpeedMultiplier.ToString(CultureInfo.InvariantCulture)}";
            void AssertProperties(TiaSpeech step)
            {
                Assert.AreEqual(richText, step.TmpRichText);
                Assert.AreEqual(typingSpeedMultiplier, step.TypingSpeedMultiplier);
            }
            AssertActionStep<TiaSpeech>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeechStepWithModal()
        {
            var richText = "I <size=200%>will</size> say something!";
            var deserializedAction = $@"
!Speech
Text: {richText}
Modal: No";
            void AssertProperties(TiaSpeech step)
            {
                Assert.AreEqual(richText, step.TmpRichText);
                Assert.IsFalse(step.IsModal);
            }
            AssertActionStep<TiaSpeech>(deserializedAction, AssertProperties);
        }

        /// <summary>
        /// Formulates a small TIA script that contains an action of type
        /// <typeparamref name="TAction"/> and asserts that the script deserializes
        /// well. You can assert the action's own property serialization in
        /// <paramref name="assertProperties"/>.
        /// </summary>
        private void AssertActionStep<TAction>(
            string deserializedAction,
            Action<TAction> assertProperties)
            where TAction : ITiaAction
        {
            var actor = "Test Actor";
            var serialized = $@"
---
Steps:
- Sequences:
  - Actor: {actor}
    Actions:
    - {deserializedAction.Replace("\n", "\n      ")}"; // Ensure proper YAML indentation.
            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(1, tiaScript.Steps.Length);

            var tiaStep = tiaScript.Steps[0];
            Assert.AreEqual(1, tiaStep.Sequences.Length);

            var tiaSequence = tiaStep.Sequences[0];
            Assert.AreEqual(actor, tiaSequence.Actor.GameObjectName);
            Assert.NotNull(tiaSequence.Actions, nameof(tiaSequence.Actions));
            Assert.AreEqual(1, tiaSequence.Actions.Length, nameof(tiaSequence.Actions));
            Assert.IsInstanceOf<TAction>(tiaSequence.Actions[0]);

            assertProperties((TAction)tiaSequence.Actions[0]);
        }
    }
}
