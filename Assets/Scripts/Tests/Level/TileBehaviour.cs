using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TileBehaviour : LevelSceneSetUp
    {

        [UnityTest]
        public IEnumerator TileGoesDown()
        {
            yield return SetUp("Test/pool");

            var tiles = root.GetComponentInChildren<Board>().tiles;

            var tile = tiles[0,1].GetComponent<Tile>();
            var initialPosition = tile.transform.position;

            tile.LowerTile();
            yield return new WaitForSeconds(tile.stateChangeDuration);

            Assert.AreNotEqual(initialPosition, tile.transform.position, "Tile did not move");
            Assert.AreEqual(tile.tileStatePositions[TileState.Down], tile.transform.position, "Tile is not at the correct position");

        }

        [UnityTest]
        public IEnumerator TileDisappears()
        {
            yield return SetUp("Test/pool");

            var player = root.GetComponentInChildren<Bear>();
            var tiles = root.GetComponentInChildren<Board>().tiles;
            var playerTile = tiles[player.logicalPosition[0], player.logicalPosition[1]];

            var board = root.GetComponentInChildren<Board>();
            
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);

            player.Move(Direction.East);

            yield return new WaitForSeconds(player.moveDuration + 0.5f);
            Assert.True(null == playerTile, "Tile remained");
        }
    }
}
