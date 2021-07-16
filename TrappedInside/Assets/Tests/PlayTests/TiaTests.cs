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

        private class TiaAsyncStart : ITiaAction
        {
            public string DebugName { get; set; }
            public bool IsDone => false;
            public GameObject ContextActor { get; private set; }

            private Task startTask;

            public void Start(ITiaActionContext context)
            {
                startTask = StartAsync(context);
            }

            private async Task StartAsync(ITiaActionContext context)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.1f));
                ContextActor = context.Actor;
            }

            public void Update(ITiaActionContext context) { }
            public void Finish(ITiaActionContext context) { }
        }

        /// <summary>
        /// Reproduces a bug: When a step had two action sequences, and the first one
        /// started a TiaSpeech action, which internally starts a background task, then by
        /// the time the task got to referencing its actor, the second action sequence
        /// which accidentally shared the same context object had changed the actor to
        /// another one, so the speech bubble appeared on the wrong actor.
        /// </summary>
        [UnityTest]
        public IEnumerator ActionSequenceStatesAreDistinct()
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
                        new TiaAsyncStart(),
                    }
                },
                new TiaActionSequence
                {
                    Actor = testObject2.name,
                    Actions = new ITiaAction[0]
                },
            });

            yield return new WaitForSeconds(0.2f);
            var asyncStart = (TiaAsyncStart)tiaPlayer.script.Steps[0].Sequences[0].Actions[0];
            Assert.AreEqual(testObject1, asyncStart.ContextActor);
        }
    }
}
