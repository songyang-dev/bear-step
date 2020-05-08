using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CameraResponse
    {
        GameObject cam;
        GameManager gameManager;

        [SetUp]
        public void SetUp()
        {
            cam = GameObject.Find("Main Camera");
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        [UnityTearDown]
        public IEnumerator Clean()
        {
            gameManager.TearDown();
            yield return null;
        }

        /// <summary>
        /// Tests if the inner function of moving the camera works
        /// </summary>
        [Test]
        public void Move()
        {
            // Use the Assert class to test conditions

            // Create level
            gameManager.Level("Test/pool");
            
            var initial = cam.transform.position;

            // Controls
            // Simulate move
            cam.GetComponent<CameraController>().Move(Vector2.right);
            Assert.AreNotEqual(cam.transform.position, initial, "Camera did not move");
        }

        /// <summary>
        /// Tests if the inner function of centering the camera works
        /// </summary>
        [Test]
        public void Center()
        {
            gameManager.Level("Test/pool");

            var initial = cam.transform.position;

            // Check if initial centering worked
            cam.GetComponent<CameraController>().Center();
            Assert.AreEqual(cam.transform.position, initial, "Camera did not center");
        }
    }
}
