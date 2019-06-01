using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public static class AssertEx
    {
        public static void AreEqual(Vector2 expected, Vector2 actual, double delta)
        {
            Assert.AreEqual(expected.x, actual.x, delta, $"Expected {expected}, got {actual}");
            Assert.AreEqual(expected.y, actual.y, delta, $"Expected {expected}, got {actual}");
        }
    }
}
