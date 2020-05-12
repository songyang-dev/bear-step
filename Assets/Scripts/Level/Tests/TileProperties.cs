using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TileProperties
    {
        GameManager gameManager;

        Board board;

        [SetUp]
        public void SetUp()
        {
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            board = GameObject.Find("Board").GetComponent<Board>();
        }

        [UnityTearDown]
        public IEnumerator Clean()
        {
            gameManager.TearDown();
            yield return null;
        }

        [UnityTest]
        public IEnumerator FlipPositions()
        {
            gameManager.Level("Test/cross");
            yield return null;

            var tile = board.GetComponent<Board>().tiles[1,1].GetComponent<Tile>();

            var initialPosition = tile.transform.position;

            Assert.AreEqual(TileState.Up, tile.State, "Tile is not UP");

            Assert.AreEqual(initialPosition, tile.tileStatePositions[tile.State], "Tile position for up state is wrong");

            Assert.AreEqual(initialPosition + new Vector3(0,0,1), tile.tileStatePositions[TileState.Down], "Tile position for down state is wrong");

            Assert.AreEqual(initialPosition + new Vector3(0,0,1), tile.FlipPosition(), "Returned flip position is wrong");
        }
    }
}
