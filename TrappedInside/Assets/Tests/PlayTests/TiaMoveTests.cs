using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaMoveTests : TiaTestBase
    {
        [UnityTest]
        public IEnumerator Move()
        {
            const float Epsilon = 0.5f;

            var tiaRoot = NewGameObject("TIA root");

            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var curveObject = NewGameObject("Test Curve");
            curveObject.transform.parent = tiaRoot.transform;
            var curve = curveObject.AddComponent<BezierCurve>();
            curve.AddPointAt(new Vector3(10, 0));
            curve.AddPointAt(new Vector3(0, 10));

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaMove
                {
                    DurationSeconds = 2,
                    CurveName = curveObject.name,
                });

            yield return new EnterPlayMode();
            AssertEx.AreEqual(new Vector3(10, 0), testObject.transform.position, Epsilon);

            yield return new WaitForSeconds(1);
            AssertEx.AreEqual(new Vector3(5, 5), testObject.transform.position, Epsilon);

            yield return new WaitForSeconds(1);
            AssertEx.AreEqual(new Vector3(0, 10), testObject.transform.position, Epsilon);
        }

        [UnityTest]
        public IEnumerator MoveLeftFlipsActor()
        {
            var tiaRoot = NewGameObject("TIA root");

            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var curveObject = NewGameObject("Test Curve");
            curveObject.transform.parent = tiaRoot.transform;
            var curve = curveObject.AddComponent<BezierCurve>();
            curve.AddPointAt(new Vector3(0, 0));
            curve.AddPointAt(new Vector3(10, 10));
            curve.AddPointAt(new Vector3(0, 20));

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaMove
                {
                    DurationSeconds = 2,
                    CurveName = curveObject.name,
                    FlipLeft = true,
                });

            const float Epsilon = 1e-4f;

            yield return new WaitForSeconds(0.5f);
            Assert.AreEqual(1, testObject.transform.localScale.x, Epsilon);

            yield return new WaitForSeconds(1);
            Assert.AreEqual(-1, testObject.transform.localScale.x, Epsilon);
        }
    }
}
