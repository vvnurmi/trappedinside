using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor.Animations;
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

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = new TiaScript
            {
                ScriptName = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = new[]
                        {
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = testObject.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaPause { DurationSeconds = 1 },
                                    new TiaActivation { activated = true },
                                    new TiaPause { DurationSeconds = 1 },
                                    new TiaActivation { activated = false },
                                }
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

        [UnityTest]
        public IEnumerator SimultaneousActors()
        {
            var tiaRoot = new GameObject("TIA root");
            var testObject1 = new GameObject("test object 1");
            var testObject2 = new GameObject("test object 2");
            testObject1.transform.parent = tiaRoot.transform;
            testObject2.transform.parent = tiaRoot.transform;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = new TiaScript
            {
                ScriptName = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = new[]
                        {
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = testObject1.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaPause { DurationSeconds = 2 },
                                    new TiaActivation { activated = true },
                                }
                            },
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = testObject2.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaPause { DurationSeconds = 1 },
                                    new TiaActivation { activated = true },
                                }
                            },
                        }
                    }
                }
            };

            yield return new EnterPlayMode();
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
            var tiaRoot = new GameObject("TIA root");
            var fakeObject = new GameObject("not under TIA root");

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = new TiaScript
            {
                ScriptName = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = new[]
                        {
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = fakeObject.name },
                                Actions = new ITiaAction[0],
                            }
                        }
                    }
                }
            };

            yield return new EnterPlayMode();

            LogAssert.Expect(LogType.Assert, new Regex($"{nameof(TiaActor)} couldn't find '{fakeObject.name}' under .*"));
            yield return new WaitForSeconds(0.5f);
        }

        [UnityTest]
        public IEnumerator Move()
        {
            const float Epsilon = 0.5f;

            var tiaRoot = new GameObject("TIA root");
            var testObject = new GameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var curve = tiaRoot.AddComponent<BezierCurve>();
            curve.AddPointAt(new Vector3(10, 0));
            curve.AddPointAt(new Vector3(0, 10));

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = new TiaScript
            {
                ScriptName = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = new[]
                        {
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = testObject.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaMove
                                    {
                                        DurationSeconds = 2,
                                        Curve = curve,
                                    }
                                }
                            }
                        }
                    }
                }
            };

            yield return new EnterPlayMode();
            AssertEx.AreEqual(new Vector3(10, 0), testObject.transform.position, Epsilon);

            yield return new WaitForSeconds(1);
            AssertEx.AreEqual(new Vector3(5, 5), testObject.transform.position, Epsilon);

            yield return new WaitForSeconds(1);
            AssertEx.AreEqual(new Vector3(0, 10), testObject.transform.position, Epsilon);
        }

        [UnityTest]
        public IEnumerator Animation()
        {
            const string DefaultStateName = "state1";
            const string AnotherStateName = "state2";

            var tiaRoot = new GameObject("TIA root");
            var testObject = new GameObject("test object");
            testObject.transform.parent = tiaRoot.transform;
            var animator = testObject.AddComponent<Animator>();
            var animatorController = new AnimatorController();
            var defaultAnimatorState = new ChildAnimatorState { state = new AnimatorState { name = DefaultStateName } };
            var anotherAnimatorState = new ChildAnimatorState { state = new AnimatorState { name = AnotherStateName } };
            var animatorControllerLayer = new AnimatorControllerLayer
            {
                name = "Base Layer",
                stateMachine = new AnimatorStateMachine
                {
                    states = new[]
                    {
                        defaultAnimatorState,
                        anotherAnimatorState,
                    },
                    defaultState = defaultAnimatorState.state,
                },
                
            };
            animatorController.AddLayer(animatorControllerLayer);
            animator.runtimeAnimatorController = animatorController;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = new TiaScript
            {
                ScriptName = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = new[]
                        {
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { gameObjectName = testObject.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaAnimation { AnimationName = AnotherStateName },
                                }
                            }
                        }
                    }
                }
            };

            void AssertAnimationState(string expectedStateName) =>
                Assert.That(
                    animator.GetCurrentAnimatorStateInfo(0).IsName(expectedStateName),
                    $"Expected animator state '{expectedStateName}' but was hash {animator.GetCurrentAnimatorStateInfo(0).shortNameHash}.");

            AssertAnimationState(DefaultStateName);
            yield return new EnterPlayMode();
            
            yield return new WaitForSeconds(0.5f);
            AssertAnimationState(AnotherStateName);
        }
    }
}
