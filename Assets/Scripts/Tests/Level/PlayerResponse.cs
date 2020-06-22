using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PlayerControl : LevelSceneSetUp
    {

        [UnityTest]
        public IEnumerator PlayerMoveSimple()
        {
            yield return SetUp("Test/pool");

            var player = root.GetComponentInChildren<Bear>();
            Assert.NotNull(player, "No player found");

            // move right, down, left
            foreach (var dir in new Direction[] { Direction.East, Direction.South, Direction.West })
            {
                var initialPosition = player.transform.position;

                player.Move(dir);
                yield return null;

                Assert.AreNotEqual(initialPosition, player.transform.position, "Player did not move");

                yield return new WaitForSeconds(player.MoveDuration);

                Assert.AreEqual(initialPosition + player.directions[dir], player.transform.position, $"Player did not move {dir}");
            }

        }

        [UnityTest]
        public IEnumerator PlayerMoveIllegal()
        {
            yield return SetUp("Test/cross");

            var player = root.GetComponentInChildren<Bear>();
            Assert.NotNull(player, "No player found");

            player.Move(Direction.South);
            yield return new WaitForSeconds(player.MoveDuration);
            
            var initialPosition = player.transform.position;

            player.Move(Direction.North);
            yield return new WaitForSeconds(player.MoveDuration);

            Assert.AreEqual(initialPosition, player.transform.position, "Player moved when it shouldn't");
        }

        [UnityTest]
        public IEnumerator FlipSimple()
        {
            yield return SetUp("Test/cross");

            var board = root.GetComponentInChildren<Board>();
            Assert.NotNull(board, "No board found");

            var player = root.GetComponentInChildren<Bear>();
            Assert.NotNull(player, "No player found");

            var playerTileState = board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State;

            var neighborTileState = board.tiles[player.LogicalPosition[0], player.LogicalPosition[1] + 1]
                .GetComponent<Tile>().State;

            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);

            Assert.AreEqual(playerTileState, board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State, "Player tile state changed when flipping");

            Assert.AreNotEqual(neighborTileState, board.tiles[player.LogicalPosition[0], player.LogicalPosition[1] + 1]
                .GetComponent<Tile>().State, "Neighbor tile did not flip");
            
        }

        [UnityTest]
        public IEnumerator FlipOnOrbTile()
        {
            yield return SetUp("Test/pool");

            var board = root.GetComponentInChildren<Board>();
            Assert.NotNull(board, "No board found");

            var player = root.GetComponentInChildren<Bear>();
            Assert.NotNull(player, "No player found");

            for (int i = 0; i < 3; i++)
            {
                player.Move(Direction.South);
                yield return new WaitForSeconds(player.MoveDuration);
            }

            var playerTileState = board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State;

            var neighborTileState = board.tiles[player.LogicalPosition[0] + 1, player.LogicalPosition[1]]
                .GetComponent<Tile>().State;
            
            yield return new WaitForSeconds(board.tiles[0,0].GetComponent<Tile>().StateChangeDuration);
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);

            Assert.AreEqual(playerTileState, board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State, "Player tile state changed when flipping");

            Assert.AreNotEqual(neighborTileState, board.tiles[player.LogicalPosition[0] + 1, player.LogicalPosition[1]]
                .GetComponent<Tile>().State, "Neighbor tile did not flip");
        }

        [UnityTest]
        public IEnumerator FlipTwice()
        {
            yield return SetUp("Test/pool");

            var board = root.GetComponentInChildren<Board>();
            Assert.NotNull(board, "No board found");

            var player = root.GetComponentInChildren<Bear>();
            Assert.NotNull(player, "No player found");

            var playerTileState = board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State;

            var neighborTileState = board.tiles[player.LogicalPosition[0], player.LogicalPosition[1] + 1]
                .GetComponent<Tile>().State;

            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);

            Assert.AreEqual(playerTileState, board.tiles[player.LogicalPosition[0], player.LogicalPosition[1]]
                .GetComponent<Tile>().State, "Player tile state changed when flipping");

            Assert.AreEqual(neighborTileState, board.tiles[player.LogicalPosition[0], player.LogicalPosition[1] + 1]
                .GetComponent<Tile>().State, "Neighbor tile did not flip");
        }

    }
}
