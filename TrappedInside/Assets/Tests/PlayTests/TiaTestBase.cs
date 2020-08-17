using NUnit.Framework;
using System.Collections.Generic;
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

        /// <summary>
        /// Simulates pressing space on a keyboard.
        /// </summary>
        protected void PressSpace()
        {
            var keyboard = InputSystem.AddDevice<Keyboard>();
            input.PressAndRelease(keyboard.spaceKey);
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
