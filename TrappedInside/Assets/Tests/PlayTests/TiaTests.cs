using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaTests
    {
        [UnityTest]
        public IEnumerator ActivationAndPause()
        {
            var tiaRoot = new GameObject("TIA root");
            var testObject = new GameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var script = tiaRoot.AddComponent<TiaScript>();
            script.scriptName = "Test Script";
            script.playOnStart = true;
            script.steps = new[]
            {
                new TiaStep
                {
                    sequences = new[]
                    {
                        new TiaActionSequence
                        {
                            actor = new TiaActor { gameObjectName = testObject.name },
                            actions = new ITiaAction[]
                            {
                                new TiaPause { durationSeconds = 1 },
                                new TiaActivation { activated = true },
                                new TiaPause { durationSeconds = 1 },
                                new TiaActivation { activated = false },
                            }
                        }
                    }
                }
            };

            yield return new EnterPlayMode();
            testObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            Assert.IsFalse(testObject.activeSelf);

            yield return new WaitForSeconds(1);
            Assert.IsTrue(testObject.activeSelf);

            yield return new WaitForSeconds(1);
            Assert.IsFalse(testObject.activeSelf);
        }
    }
}
