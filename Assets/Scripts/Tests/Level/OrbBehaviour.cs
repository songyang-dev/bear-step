using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class OrbBehaviour : LevelSceneSetUp
    {
        [UnityTest]
        public IEnumerator Collect()
        {
            yield return SetUp("Test/pool");

            var orbs = root.GetComponentInChildren<Board>().orbs;

            orbs[0].GetComponent<Orb>().Touched();
            yield return null;

            Assert.True(null == orbs[0], "Orb remained");
        }

        [UnityTest]
        public IEnumerator Lowers()
        {
            yield return SetUp("Test/pool");
            
            var board = root.GetComponentInChildren<Board>();
            var orbs = board.orbs;

            var initialPosition = orbs[0].transform.position;
            var correctTargetPosition = orbs[0].GetComponent<Orb>().transform.position + new Vector3(0,0,1);

            board.Flip();
            yield return new WaitForSeconds(board.flipDuration);

            Assert.AreNotEqual(initialPosition, orbs[0].transform.position, "Orb did not lower");
            Assert.AreEqual(correctTargetPosition, orbs[0].transform.position, "Orb is in the wrong position");
        }
    }
}
