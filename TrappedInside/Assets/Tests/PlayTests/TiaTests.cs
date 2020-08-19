using NUnit.Framework;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TiaTests : TiaTestBase
    {
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
            tiaPlayer.script = NewMultiSequenceScript(new[]
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
            tiaPlayer.script = NewSimpleScript(fakeObject);

            LogAssert.Expect(LogType.Assert, new Regex($"{nameof(TiaActor)} couldn't find '{fakeObject.name}' under .*"));
            yield return new WaitForSeconds(0.5f);
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
