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

                tile.ChangeTileState(TileState.Down);
                yield return null;

                Assert.AreNotEqual(initial, tile.transform.position, "Tile did not move");

                yield return new WaitForSeconds(tile.moveDuration);

                Assert.AreEqual(initial + new Vector3(0,0,1), tile.transform.position, "Tile did not move down");
            }
            
        }
    }
}
