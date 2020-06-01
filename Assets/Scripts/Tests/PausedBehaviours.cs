using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEngine;

namespace Tests
{
    public class PausedBehaviours: LevelSceneSetUp
    {
        [UnityTest]
        public IEnumerator MenuAppearance()
        {
            yield return SetUp("Test/pool");

            var pauseMenu = root.GetComponentInChildren<PauseMenu>();
            
            // get the ui panel that is disabled by default
            var menuObject = pauseMenu.transform.GetChild(0).gameObject; // assuming that it is the only child

            Assert.False(pauseMenu.GameIsPaused, "Game is paused initially");
            
            pauseMenu.Pause();
            yield return null;

            Assert.AreEqual(0, Time.timeScale, "Time not frozen");
            Assert.True(menuObject.activeInHierarchy, "Menu does not appear");

            pauseMenu.Resume();
            yield return null;

            Assert.AreEqual(1, Time.timeScale, "Time not unfrozen");
            Assert.False(menuObject.activeInHierarchy, "Menu still present");
        }
    }
}