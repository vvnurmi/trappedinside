using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaTests
    {
        private int gameObjectsAtSetup;
        private List<GameObject> createdGameObjects = new List<GameObject>();

        private GameObject NewGameObject(string name)
        {
            var gameObject = new GameObject(name);
            createdGameObjects.Add(gameObject);
            return gameObject;
        }

        /// <summary>
        /// Creates a test <see cref="TiaScript"/> with one <see cref="TiaStep"/>
        /// that contains one <see cref="TiaActionSequence"/> that contains the
        /// given <paramref name="actions"/>.
        /// </summary>
        private TiaScript NewSimpleScript(GameObject actor, params ITiaAction[] actions) =>
            new TiaScript
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
                                Actor = new TiaActor { GameObjectName = actor.name },
                                Actions = actions,
                            }
                        }
                    }
                }
            };

        [SetUp]
        public void Setup()
        {
            gameObjectsAtSetup = Object.FindObjectsOfType<GameObject>().Length;
        }

        [TearDown]
        public void Teardown()
        {
            Debug.Log($"Deleting {createdGameObjects.Count} game objects that were created during the test.");
            foreach (var gameObject in createdGameObjects)
                Object.DestroyImmediate(gameObject);
            createdGameObjects.Clear();

            // Object count safety check to guard against leaking objects that could break tests.
            var gameObjectsAtTeardown = Object.FindObjectsOfType<GameObject>().Length;
            Assert.AreEqual(gameObjectsAtSetup, gameObjectsAtTeardown, "Game objects may be leaking from test to another");
        }

        [UnityTest]
        public IEnumerator ActivationAndPause()
        {
            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject, 
                new TiaPause { DurationSeconds = 1 },
                new TiaActivate(),
                new TiaPause { DurationSeconds = 1 },
                new TiaDeactivate());

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
            var tiaRoot = NewGameObject("TIA root");
            var testObject1 = NewGameObject("test object 1");
            var testObject2 = NewGameObject("test object 2");
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
                                Actor = new TiaActor { GameObjectName = testObject1.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaPause { DurationSeconds = 2 },
                                    new TiaActivate(),
                                }
                            },
                            new TiaActionSequence
                            {
                                Actor = new TiaActor { GameObjectName = testObject2.name },
                                Actions = new ITiaAction[]
                                {
                                    new TiaPause { DurationSeconds = 1 },
                                    new TiaActivate(),
                                }
                            },
                        }
                    }
                }
            };

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
            tiaPlayer.script = NewSimpleScript(fakeObject);

            LogAssert.Expect(LogType.Assert, new Regex($"{nameof(TiaActor)} couldn't find '{fakeObject.name}' under .*"));
            yield return new WaitForSeconds(0.5f);
        }

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
        public IEnumerator Animation()
        {
            const string DefaultStateName = "state1";
            const string AnotherStateName = "state2";

            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
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
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaAnimation { AnimationName = AnotherStateName });

            void AssertAnimationState(string expectedStateName) =>
                Assert.That(
                    animator.GetCurrentAnimatorStateInfo(0).IsName(expectedStateName),
                    $"Expected animator state '{expectedStateName}' but was hash {animator.GetCurrentAnimatorStateInfo(0).shortNameHash}.");

            AssertAnimationState(DefaultStateName);
            
            yield return new WaitForSeconds(0.5f);
            AssertAnimationState(AnotherStateName);
        }
    }
}
