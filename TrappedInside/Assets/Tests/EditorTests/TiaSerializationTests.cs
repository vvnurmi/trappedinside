﻿using NUnit.Framework;
using System;
using System.Globalization;

namespace Tests
{
    public class TiaSerializationTests
    {
        [Test]
        public void BasicProperties()
        {
            var description = "Test Script";
            var playOnStart = true;

            var serialized = $@"
---
Description: {description}
AutoPlay: {playOnStart}";

            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(description, tiaScript.Description);
            Assert.AreEqual(playOnStart, tiaScript.PlayOnStart);
        }

        [Test]
        public void ActivateAndDeactivate()
        {
            AssertAction<TiaActivate>("!Activate", action => { });
            AssertAction<TiaDeactivate>("!Deactivate", action => { });
        }

        [Test]
        public void Animate()
        {
            var animationName = "Test Animation";
            var deserializedAction = $@"
!Animate
Name: {animationName}";

            void AssertProperties(TiaAnimate action)
            {
                Assert.AreEqual(animationName, action.AnimationName);
            }
            AssertAction<TiaAnimate>(deserializedAction, AssertProperties);
        }

        [Test]
        public void Move()
        {
            var CloseEnough = 1e-6;
            var durationSeconds = 2.5f;
            var curveName = "Test Curve";
            var deserializedAction = $@"
!Move
Curve: {curveName}
Seconds: {durationSeconds.ToString(CultureInfo.InvariantCulture)}";

            void AssertProperties(TiaMove action)
            {
                Assert.AreEqual(curveName, action.CurveName);
                Assert.AreEqual(durationSeconds, action.DurationSeconds, CloseEnough);
            }
            AssertAction<TiaMove>(deserializedAction, AssertProperties);
        }

        [Test]
        public void MoveWithFlip()
        {
            var deserializedAction = $@"
!Move
FlipLeft: true
LooksLeftInitially: true";
            void AssertProperties(TiaMove action)
            {
                Assert.IsTrue(action.FlipLeft);
                Assert.IsTrue(action.LooksLeftInitially);
            }
            AssertAction<TiaMove>(deserializedAction, AssertProperties);
        }

        [Test]
        public void Pause()
        {
            var pauseSeconds = 2.5f;
            var deserializedAction = $@"
!Pause
Seconds: {pauseSeconds.ToString(CultureInfo.InvariantCulture)}";

            void AssertProperties(TiaPause action)
            {
                Assert.AreEqual(pauseSeconds, action.DurationSeconds);
            }
            AssertAction<TiaPause>(deserializedAction, AssertProperties);
        }

        [Test]
        public void PlayScript()
        {
            var scriptName = "Script To Play";
            var deserializedAction = $@"
!PlayScript
Name: {scriptName}";
            void AssertProperties(TiaPlayScript action)
            {
                Assert.AreEqual(scriptName, action.ScriptName);
            }
            AssertAction<TiaPlayScript>(deserializedAction, AssertProperties);
        }

        [Test]
        public void Speak()
        {
            var richText = "I <size=200%>will</size> say something!";
            var bubbleName = "Speech Bubble";
            var deserializedAction = $@"
!Speak
Text: {richText}
Bubble: {bubbleName}";
            void AssertProperties(TiaSpeak action)
            {
                Assert.AreEqual(richText, action.TmpRichText);
                Assert.AreEqual(bubbleName, action.SpeechBubbleName);
                Assert.AreEqual(1, action.TypingSpeedMultiplier);
                Assert.IsTrue(action.IsModal);
            }
            AssertAction<TiaSpeak>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeakWithSpeed()
        {
            var richText = "I <size=200%>will</size> say something!";
            var typingSpeedMultiplier = 1.2f;
            var deserializedAction = $@"
!Speak
Text: {richText}
Speed: {typingSpeedMultiplier.ToString(CultureInfo.InvariantCulture)}";
            void AssertProperties(TiaSpeak action)
            {
                Assert.AreEqual(richText, action.TmpRichText);
                Assert.AreEqual(typingSpeedMultiplier, action.TypingSpeedMultiplier);
            }
            AssertAction<TiaSpeak>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeakWithModal()
        {
            var richText = "I <size=200%>will</size> say something!";
            var deserializedAction = $@"
!Speak
Text: {richText}
Modal: No";
            void AssertProperties(TiaSpeak action)
            {
                Assert.AreEqual(richText, action.TmpRichText);
                Assert.IsFalse(action.IsModal);
            }
            AssertAction<TiaSpeak>(deserializedAction, AssertProperties);
        }

        [Test]
        public void SpeakWithChoice()
        {
            var leftChoice = "Yes";
            var rightChoice = "No";
            var deserializedAction = $@"
!Speak
Left: {leftChoice}
Right: {rightChoice}";
            void AssertProperties(TiaSpeak action)
            {
                Assert.AreEqual(leftChoice, action.LeftChoice);
                Assert.AreEqual(rightChoice, action.RightChoice);
            }
            AssertAction<TiaSpeak>(deserializedAction, AssertProperties);
        }

        [Test]
        public void Invoke()
        {
            var methodName = "DoSomething";
            var deserializedAction = $@"
!Invoke
Name: {methodName}";
            void AssertProperties(TiaInvoke action)
            {
                Assert.AreEqual(methodName, action.MethodName);
            }
            AssertAction<TiaInvoke>(deserializedAction, AssertProperties);
        }

        /// <summary>
        /// Formulates a small TIA script that contains an action of type
        /// <typeparamref name="TAction"/> and asserts that the script deserializes
        /// well. You can assert the action's own property serialization in
        /// <paramref name="assertProperties"/>.
        /// </summary>
        private void AssertAction<TAction>(
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
            Assert.AreEqual(actor, tiaSequence.Actor);
            Assert.NotNull(tiaSequence.Actions, nameof(tiaSequence.Actions));
            Assert.AreEqual(1, tiaSequence.Actions.Length, nameof(tiaSequence.Actions));
            Assert.IsInstanceOf<TAction>(tiaSequence.Actions[0]);

            assertProperties((TAction)tiaSequence.Actions[0]);
        }
    }
}
