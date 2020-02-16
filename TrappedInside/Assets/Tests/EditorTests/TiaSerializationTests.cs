using NUnit.Framework;

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
ScriptName: {scriptName}
PlayOnStart: {playOnStart}";

            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(scriptName, tiaScript.ScriptName);
            Assert.AreEqual(playOnStart, tiaScript.PlayOnStart);
        }
    }
}
