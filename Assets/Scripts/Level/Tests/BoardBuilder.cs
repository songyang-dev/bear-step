using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class BoardBuilder
    {

        protected GameObject board;

        protected GameManager gameManager;

        [SetUp]
        public void PrepareTest()
        {
            board = GameObject.Find("Board");
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        }

        [UnityTearDown]
        public IEnumerator Clean()
        {
            gameManager.TearDown();
            yield return null;
        }

        /// <summary>
        /// Tests if the 1x1 level is built properly
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator Build1x1()
        {
            // Use the Assert class to test conditions
            
            // Create level
            gameManager.Level("Test/1x1");
            yield return null;

            var boardScript = board.GetComponent<Board>();

            // general quantities
            Assert.AreEqual(1,boardScript.tiles.Length,"Invalid number of tiles");
            Assert.AreEqual(2,board.transform.childCount, "Invalid number of children");
            Assert.AreEqual(0,boardScript.orbs.Length, "Invalid number of orbs");
            Assert.AreEqual(1,boardScript.characters.Length, "Invalid number of characters");

            // element locations
            Assert.AreEqual(Vector3.zero, boardScript.tiles[0,0].transform.position, "Tile not at origin");
            Assert.AreEqual(Vector3.zero, boardScript.player.transform.position, "Player not at correct position");
        }

        /// <summary>
        /// Tests if the cross level is built properly
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator BuildCross()
        {
            // Create level
            gameManager.Level("Test/cross");
            yield return null;
            
            var boardScript = board.GetComponent<Board>();

            Assert.AreEqual(12,boardScript.tiles.Length,"Invalid number of tiles");
            Assert.AreEqual(7,board.transform.childCount, "Invalid number of children");
            Assert.AreEqual(1,boardScript.orbs.Length, "Invalid number of orbs");
            Assert.AreEqual(1,boardScript.characters.Length, "Invalid number of characters");

            Assert.AreEqual(new Vector3(1,3,0), boardScript.tiles[1,0].transform.position, "Tile not at correct position");
            Assert.AreEqual(new Vector3(1,3,0), boardScript.player.transform.position, "Player not at correct position");
        }

        /// <summary>
        /// Tests if the pool level is built properly
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator BuildPool()
        {
            // Create level
            gameManager.Level("Test/pool");
            yield return null;

            var boardScript = board.GetComponent<Board>();

            Assert.AreEqual(28,boardScript.tiles.Length,"Invalid number of tiles");
            Assert.AreEqual(21,board.transform.childCount, "Invalid number of children");
            Assert.AreEqual(3,boardScript.orbs.Length, "Invalid number of orbs");
            Assert.AreEqual(1,boardScript.characters.Length, "Invalid number of characters");

            Assert.AreEqual(new Vector3(0,3,0), boardScript.player.transform.position, "Player not at correct position");

            var tileWorldPositions = new Vector3[] {
                new Vector3(0,3,0), new Vector3(1,3,0), new Vector3(2,3,0), new Vector3(3,3,0), new Vector3(4,3,0), new Vector3(5,3,0), new Vector3(6,3,0),
                new Vector3(0,2,0), new Vector3(1,2,0), new Vector3(6,2,0),
                new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(6,1,0),
                new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0), new Vector3(3,0,0), new Vector3(4,0,0), new Vector3(5,0,0), new Vector3(6,0,0)
            };

            foreach (var tile in tileWorldPositions)
            {
                Assert.AreEqual(tile, 
                    boardScript.tiles[(int) tile.x, (int) (boardScript.boardDimension[1] - 1 - tile.y)].transform.position,
                    "Tile not at correct position");
            }
        }
    }
}
