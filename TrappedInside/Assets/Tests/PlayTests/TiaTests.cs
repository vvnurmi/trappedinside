using NUnit.Framework;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaTests : TiaTestBase
    {
        [UnityTest]
        public IEnumerator SimultaneousActors()
        {
            var tiaRoot = NewGameObject("TIA root");
            var testObject1 = NewGameObject("test object 1");
            var testObject2 = NewGameObject("test object 2");
            testObject1.transform.parent = tiaRoot.transform;
            testObject2.transform.parent = tiaRoot.transform;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewMultiSequenceScript(new[]
            {
                new TiaActionSequence
                {
                    Actor = testObject1.name,
                    Actions = new ITiaAction[]
                    {
                        new TiaPause { DurationSeconds = 2 },
                        new TiaActivate(),
                    }
                },
                new TiaActionSequence
                {
                    Actor = testObject2.name,
                    Actions = new ITiaAction[]
                    {
                        new TiaPause { DurationSeconds = 1 },
                        new TiaActivate(),
                    }
                },
            });

            testObject1.SetActive(false);
            testObject2.SetActive(false);

            yield return new WaitForSeconds(0.5f);
            Assert.IsFalse(testObject1.activeSelf);
            Assert.IsFalse(testObject2.activeSelf);

            yield return new WaitForSeconds(1);
            Assert.IsFalse(testObject1.activeSelf);
            Assert.IsTrue(testObject2.activeSelf);

            yield return new WaitForSeconds(1);
            Assert.IsTrue(testObject1.activeSelf);
            Assert.IsTrue(testObject2.activeSelf);
        }

        [UnityTest]
        public IEnumerator ThrowsIfActorNotFoundUnderRoot()
        {
            var tiaRoot = NewGameObject("TIA root");
            var fakeObject = NewGameObject("not under TIA root");

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(fakeObject,
                new TiaActivate(),
                new TiaPause { DurationSeconds = 1f });

            // The script's sole step and action sequence should abort early because the actor can't be found.
            yield return new WaitForSeconds(0.1f);
            Assert.IsFalse(tiaPlayer.IsPlaying);
        }

        /// <summary>
        /// Verify that the state of execution of a TIA script is not stored in the
        /// <see cref="TiaScript"/> instance, so that the same script can be played
        /// simultaenously multiple times.
        /// </summary>
        [UnityTest]
        public IEnumerator ScriptIsStateless()
        {
            const string TestValue1 = "test value 1";
            const string TestValue2 = "test value 2";

            var script = NewSimpleScript(actor: null,
                new TiaInvoke
                {
                    MethodName = nameof(TiaMethods.SetTestString),
                    MethodArgument1 = TestValue1,
                },
                new TiaPause { DurationSeconds = 0.1f },
                new TiaInvoke
                {
                    MethodName = nameof(TiaMethods.SetTestString),
                    MethodArgument1 = TestValue2,
                }
            );

            var tiaRoot1 = NewGameObject("TIA root 1");
            var tiaRoot2 = NewGameObject("TIA root 2");
            var tiaPlayer1 = tiaRoot1.AddComponent<TiaPlayer>();
            var tiaPlayer2 = tiaRoot2.AddComponent<TiaPlayer>();

            tiaPlayer1.Play(script);
            yield return new WaitForSeconds(0.08f);
            // Test string was last set by the first TiaInvoke played by tiaPlayer1.
            Assert.AreEqual(TestValue1, TiaMethods.testString1);

            tiaPlayer2.Play(script);
            yield return new WaitForSeconds(0.04f);

            // Test string was last set by the second TiaInvoke played by tiaPlayer1,
            // unless its script playing state was incorrectly reset by tiaPlayer2.
            Assert.AreEqual(TestValue2, TiaMethods.testString1);
        }
    }
}
