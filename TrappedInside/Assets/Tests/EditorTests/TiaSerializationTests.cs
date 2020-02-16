using NUnit.Framework;

namespace Tests
{
    public class TiaSerializationTests
    {
        [Test]
        public void Basic()
        {
            var ScriptName = "Test Script";
            var serialized = $@"
---
scriptName: {ScriptName}";

            var tiaScript = TiaScript.Read(serialized);
            Assert.AreEqual(ScriptName, tiaScript.scriptName);
        }
    }
}
