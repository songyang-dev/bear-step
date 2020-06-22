using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class WinConditions : LevelSceneSetUp
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WinMenuAppears()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return SetUp("Test/pool");

            var gm = root.GetComponentInChildren<GameManager>();

            var winMenuScript = root.GetComponentInChildren<WinMenuUI>();
            var winMenu = winMenuScript.transform.GetChild(0);

            Assert.False(winMenu.gameObject.activeInHierarchy);

            gm.WinLevel.Invoke();

            Assert.True(winMenu.gameObject.activeInHierarchy);
        }
    }
}
