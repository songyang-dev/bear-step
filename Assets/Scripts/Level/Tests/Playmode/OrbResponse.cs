using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;

namespace Tests
{
    public class SceneSetUp
    {
        protected GameManager gameManager;

        protected GameObject[] orbs;

        protected Board board;

        protected CameraController camera;

        protected Bear bear;

        protected GameObject light;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // prepare the game objects
            var gm = new GameObject("Game Manager");
            gameManager = gm.AddComponent<GameManager>();

            var board = new GameObject("Board");
            this.board = board.AddComponent<Board>();

            var camera = new GameObject("Main Camera");
            camera.AddComponent<Camera>();
            this.camera = camera.AddComponent<CameraController>();

            light = new GameObject("Directional Light");
            light.transform.position = new Vector3(0,3,0);
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
            light.AddComponent<Light>();

            // link prefabs
            var bearPrefab = Resources.Load<GameObject>("Prefabs/Bear");
            var orbPrefab = Resources.Load<GameObject>("Prefabs/Orb");
            var tilePrefab = Resources.Load<GameObject>("Prefabs/Tile");

            Assert.NotNull(bearPrefab, "Bear prefab not loaded");
            Assert.NotNull(orbPrefab, "Orb prefab not loaded");
            Assert.NotNull(tilePrefab, "Tile prefab not loaded");

            gameManager.board = board;
            gameManager.cam = camera;
            gameManager.test = true;
            gameManager.testLevel = "Test/pool";

            this.board.tilePrefab = tilePrefab;
            this.board.orbPrefab = orbPrefab;
            this.board.bearPrefab = bearPrefab;
            this.board.touchOrb = new UnityEngine.Events.UnityEvent();

            this.camera.board = board;
            this.camera.speed = 3;
            this.camera.offset = new Vector3(0.1800001f, -1.86f, -5.08f);
            this.camera.rotation = new Vector3(-37.233f, 0, 0);

            yield return null;

            this.orbs = this.board.orbs;
            this.bear = this.board.player.GetComponent<Bear>();

            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            gameManager.TearDown();
            GameObject.Destroy(this.camera.gameObject);
            GameObject.Destroy(this.board.gameObject);
            GameObject.Destroy(this.gameManager.gameObject);
            GameObject.Destroy(this.light);
            yield return null;
        }   
    }

    public class OrbResponse : SceneSetUp
    {           
        /// <summary>
        /// Tests if the orb was detected by the game when the player steps on it
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator DetectOrbAtGivenLocation()
        {
            // Use the Assert class to test conditions            
            Assert.True(this.board.PlayerMovedTo(Vector3.zero), "No contact made with an orb");
            yield return null;
            Assert.True(this.board.PlayerMovedTo(new Vector3(6, 0, 0)), "No contact made with an orb");
            yield return null;
            Assert.True(this.board.PlayerMovedTo(new Vector3(6, 3, 0)), "No contact made with an orb");
            yield return null;
            Assert.False(this.board.PlayerMovedTo(new Vector3(1,1,0)), "An orb was found where there is none");
            Assert.False(this.board.PlayerMovedTo(new Vector3(4,5,0)), "An orb was found where there is none");
            yield return null;
        }
    }
}
