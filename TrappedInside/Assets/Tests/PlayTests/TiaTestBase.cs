﻿using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tests
{
    /// <summary>
    /// Base class for TIA tests. Contains helpful common things.
    /// </summary>
    public abstract class TiaTestBase
    {
        private int gameObjectsAtSetup;
        private List<GameObject> createdGameObjects = new List<GameObject>();
        private InputTestFixture input = new InputTestFixture();
        private Keyboard mockKeyboard = InputSystem.AddDevice<Keyboard>();

        /// <summary>
        /// Creates a new game object for a test. Only create game objects with this
        /// method during tests. That way the objects will get cleaned up after the
        /// test finishes instead of bleeding to the following tests.
        /// </summary>
        protected GameObject NewGameObject(string name)
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
        protected TiaScript NewSimpleScript(GameObject actor, params ITiaAction[] actions) =>
            new TiaScript
            {
                Description = "Test Script",
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

        protected TiaScript NewMultiSequenceScript(params TiaActionSequence[] actionSequences) =>
            new TiaScript
            {
                Description = "Test Script",
                PlayOnStart = true,
                Steps = new[]
                {
                    new TiaStep
                    {
                        Sequences = actionSequences,
                    }
                }
            };

        /// <summary>
        /// Simulates pressing <paramref name="key"/> on a keyboard and releasing
        /// it instantly. Good for triggers.
        /// </summary>
        protected void PressKey(Key key)
        {
            var control = mockKeyboard.allKeys.First(keyControl => keyControl.keyCode == key);
            input.PressAndRelease(control);
        }

        /// <summary>
        /// Simulates pressing <paramref name="key"/> on a keyboard for a short while
        /// and then releasing it. Good for navigation.
        /// </summary>
        protected IEnumerator PressAndHoldKey(Key key)
        {
            var control = mockKeyboard.allKeys.First(keyControl => keyControl.keyCode == key);
            input.Press(control);
            yield return new WaitForSeconds(0.1f);
            input.Release(control);
        }

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
    }
}
