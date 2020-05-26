using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

namespace Tests
{
    [TestFixture]
    public class CameraResponse : LevelSceneSetUp
    {
        private InputTestFixture input = new InputTestFixture();

        /// <summary>
        /// Tests whether the camera responds to movement commands
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Moving()
        {
            yield return SetUp("Test/pool");

            var camera = root.GetComponentInChildren<CameraController>();


            var initialPosition = camera.transform.position;

            // go to the right
            camera.Move(Vector2.right);
            yield return null;

            Assert.AreNotEqual(initialPosition, camera.transform.position, "Camera did not move");
            Assert.Greater(camera.transform.position.x, initialPosition.x, "Camera did not move to the right");

        }

        [UnityTest]
        public IEnumerator Center()
        {
            yield return SetUp("Test/pool");

            var camera = root.GetComponentInChildren<CameraController>();

            // move to the right by a lot
            camera.Move(Vector2.right * 3);
            yield return null;

            var initialPosition = camera.transform.position;

            camera.Center();
            yield return null;

            Assert.AreNotEqual(initialPosition, camera.transform.position, "Camera did not change position");
        }
    }
}
