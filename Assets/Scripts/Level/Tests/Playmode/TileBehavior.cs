using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TileBehavior : SceneSetUp
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TileMovesDown()
        {
            var tiles = this.board.tiles;
            var i = 0;
            var numberOfTilesToCheck = 4;

            foreach (var item in tiles)
            {
                if (item == null) continue;
                if (i == numberOfTilesToCheck) break;
                i++;

                var tile = item.GetComponent<Tile>();
                var initial = tile.transform.position;

                tile.LowerTile();
                yield return null;

                Assert.AreNotEqual(initial, tile.transform.position, "Tile did not move");

                yield return new WaitForSeconds(tile.moveDuration);

                Assert.AreEqual(initial + new Vector3(0, 0, 1), tile.transform.position, "Tile did not move down");
            }

        }

        /// <summary>
        /// Checks if the orb lowers down as the tile lowers down. The test currently fails because 
        /// child objects don't update their global transform positions. But the game still works.
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator OrbMovesDown()
        {

            var rightLowerCornerTile = board.tiles[board.boardDimension[0] - 1, board.boardDimension[1] - 1];
            var rightLowerCornerTileInitialPosition = rightLowerCornerTile.transform.position;

            GameObject rightLowerCornerOrb = null;
            foreach (var orb in orbs)
            {
                if (orb.transform.position.x == rightLowerCornerTile.transform.position.x
                    && orb.transform.position.y == rightLowerCornerTile.transform.position.y)
                {
                    rightLowerCornerOrb = orb; 
                    break;
                }
            }
            Assert.NotNull(rightLowerCornerOrb, "Lower right corner orb not found");

            var orbInitialPosition = rightLowerCornerOrb.transform.position;
            rightLowerCornerTile.GetComponent<Tile>().LowerTile();
            yield return null;

            Assert.AreNotEqual(orbInitialPosition, rightLowerCornerOrb.transform.position, "Orb did not move");

            yield return new WaitForSeconds(rightLowerCornerTile.GetComponent<Tile>().moveDuration);

            Assert.AreEqual(rightLowerCornerTile.GetComponent<Tile>().tileStatePositions[TileState.Down],
                rightLowerCornerOrb.transform.position, "Right lower corner orb did not go down");
        }

        [UnityTest]
        public IEnumerator Flip()
        {
            var bearTile = board.tiles[bear.logicalPosition[0], bear.logicalPosition[1]];
            var bearTileInitialPosition = bearTile.transform.position;

            var rightLowerCornerTile = board.tiles[board.boardDimension[0] - 1, board.boardDimension[1] - 1];
            var rightLowerCornerTileInitialPosition = rightLowerCornerTile.transform.position;

            board.Flip();
            yield return null;

            // check if the player's tile has not moved
            Assert.AreEqual(bearTileInitialPosition, bearTile.transform.position, "Bear's tile moved");

            // check if other tiles moved
            Assert.AreNotEqual(rightLowerCornerTileInitialPosition, rightLowerCornerTile.transform.position, "Right lower corner tile did not move");

            yield return new WaitForSeconds(this.board.flipDuration);

            Assert.AreEqual(rightLowerCornerTileInitialPosition + new Vector3(0, 0, 1), rightLowerCornerTile.transform.position, "Right lower corner tile did not go down");

            yield return new WaitForSeconds(0.1f);

            Assert.AreEqual(rightLowerCornerTileInitialPosition + new Vector3(0, 0, 1), rightLowerCornerTile.transform.position, "Right lower corner tile moved too much");
        }

        [UnityTest]
        public IEnumerator FlipTwice()
        {
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);
            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);
        }
    }
}
