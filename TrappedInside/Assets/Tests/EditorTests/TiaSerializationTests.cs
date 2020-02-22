using NUnit.Framework;
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
        public void PauseStep()
        {
            var actor = "Test Actor";
            var pauseSeconds = 2.5f;
            var serialized = $@"
---
Steps:
- Sequences:
  - Actor: {actor}
    Actions:
    - !Pause
      Seconds: {pauseSeconds.ToString(CultureInfo.InvariantCulture)}";
            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(1, tiaScript.Steps.Length);

            var tiaStep = tiaScript.Steps[0];
            Assert.AreEqual(1, tiaStep.Sequences.Length);

            var tiaSequence = tiaStep.Sequences[0];
            Assert.AreEqual(actor, tiaSequence.Actor.GameObjectName);
            Assert.NotNull(tiaSequence.Actions, nameof(tiaSequence.Actions));
            Assert.AreEqual(1, tiaSequence.Actions.Length, nameof(tiaSequence.Actions));
            Assert.IsInstanceOf<TiaPause>(tiaSequence.Actions[0]);

            var tiaPause = (TiaPause)tiaSequence.Actions[0];
            Assert.AreEqual(2.5f, tiaPause.DurationSeconds);
        }
    }
}
