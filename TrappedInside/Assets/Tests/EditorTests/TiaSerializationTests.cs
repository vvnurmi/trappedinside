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
        public void PauseStep()
        {
            var pauseSeconds = 2.5f;
            var deserializedAction = $@"
!Pause
Seconds: {pauseSeconds.ToString(CultureInfo.InvariantCulture)}";
            void AssertProperties(TiaPause tiaPause) =>
                Assert.AreEqual(pauseSeconds, tiaPause.DurationSeconds);
            AssertActionStep<TiaPause>(deserializedAction, AssertProperties);
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
