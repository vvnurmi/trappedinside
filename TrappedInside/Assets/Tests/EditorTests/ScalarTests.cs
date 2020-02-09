using NUnit.Framework;

using Scalar = TI.Scalar;

namespace Tests
{
    public class ScalarTests
    {
        [Test]
        public void LerpTowards()
        {
            Assert.AreEqual(0, Scalar.LerpTowards(0, 0, 0));
            Assert.AreEqual(0, Scalar.LerpTowards(0, 1, 0));
            Assert.AreEqual(0, Scalar.LerpTowards(0, 0, 1));
            Assert.AreEqual(1, Scalar.LerpTowards(0, 1, 1));
            Assert.AreEqual(1, Scalar.LerpTowards(1, 0, 0));
            Assert.AreEqual(0, Scalar.LerpTowards(1, 0, 1));
            Assert.AreEqual(0.3f, Scalar.LerpTowards(0.2f, 0.5f, 0.1f));
            Assert.AreEqual(0.5f, Scalar.LerpTowards(0.2f, 0.5f, 0.4f));
            Assert.AreEqual(0.7f, Scalar.LerpTowards(0.8f, 0.5f, 0.1f));
            Assert.AreEqual(0.5f, Scalar.LerpTowards(0.8f, 0.5f, 0.4f));
        }
    }
}
