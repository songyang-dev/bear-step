using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class BoardBuilder: LevelSceneSetUp
    {
        
        private IEnumerator BuildLevel(string name, int correctTileCount, int correctOrbCount)
        {
            yield return SetUp(name);

            // check tile count
            var tileCount = root.GetComponentInChildren<Board>().tileCount;
            Assert.AreEqual(correctTileCount, tileCount, "Incorrect tile count");

            // check orb count
            var orbCount = root.GetComponentInChildren<Board>().orbs.Length;
            Assert.AreEqual(correctOrbCount, orbCount, "Incorrect orb count");
        }

        [UnityTest]
        public IEnumerator Build1x1()
        {
            yield return BuildLevel("Test/1x1", 1, 0);
        }

        [UnityTest]
        public IEnumerator BuildCross()
        {
            yield return BuildLevel("Test/cross", 6, 1);
        }

        [UnityTest]
        public IEnumerator BuildPool()
        {
            yield return BuildLevel("Test/pool", 20, 3);
        }

        [UnityTest]
        public IEnumerator BuildOrbRow()
        {
            yield return BuildLevel("Test/orbRow", 5, 4);
        }
    }
}
