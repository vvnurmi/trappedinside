using NUnit.Framework;
using System.Collections;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    /// <summary>
    /// Tests for TIA actions that only have a simple test.
    /// If there are more tests for a particular type of TIA action,
    /// please extract them to their own test class, to keep things tidy.
    /// </summary>
    public class TiaBasicActionTests : TiaTestBase
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
        public IEnumerator Animate()
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
                new TiaAnimate { AnimationName = AnotherStateName });

            void AssertAnimationState(string expectedStateName) =>
                Assert.That(
                    animator.GetCurrentAnimatorStateInfo(0).IsName(expectedStateName),
                    $"Expected animator state '{expectedStateName}' but was hash {animator.GetCurrentAnimatorStateInfo(0).shortNameHash}.");

            AssertAnimationState(DefaultStateName);

            yield return new WaitForSeconds(0.5f);
            AssertAnimationState(AnotherStateName);
        }

        [UnityTest]
        public IEnumerator Invoke()
        {
            var tiaRoot = NewGameObject("TIA root");
            var testObject = NewGameObject("test object");
            testObject.transform.parent = tiaRoot.transform;

            var tiaPlayer = tiaRoot.AddComponent<TiaPlayer>();
            tiaPlayer.script = NewSimpleScript(testObject,
                new TiaInvoke { MethodName = nameof(TiaMethods.SetTestFlagToTrue) });

            TiaMethods.testFlag = false;

            yield return new WaitForSeconds(0.1f);
            Assert.IsTrue(TiaMethods.testFlag);
        }
    }
}
