using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class PausedBehaviours: LevelSceneSetUp
    {
        [UnityTest]
        public IEnumerator MenuAppearsAndResumes()
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

        [UnityTest]
        public IEnumerator BackToMainMenu()
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

            pauseMenu.Menu();
            yield return null;

            // level scene tear down 
            //GameObject.Destroy(root);

            var thisScene = SceneManager.GetActiveScene().name;

            Assert.AreEqual("Main Menu", thisScene, "Scene did not change to Main Menu");

            MenuTearDown();
        }

        [UnityTest]
        public IEnumerator Restart()
        {
            yield return SetUp("Campaign/Tutorial");

            var pauseMenu = root.GetComponentInChildren<PauseMenu>();

            // get the ui panel that is disabled by default
            var menuObject = pauseMenu.transform.GetChild(0).gameObject; // assuming that it is the only child

            // move the player
            var player = root.GetComponentInChildren<Bear>();
            player.Move(Direction.East);

            yield return new WaitForSeconds(player.MoveDuration * 2);

            // player position to check later
            var position = player.LogicalPosition;

            Assert.False(pauseMenu.GameIsPaused, "Game is paused initially");
            
            pauseMenu.Pause();
            yield return null;

            pauseMenu.Restart();
            yield return null;

            var newPosition =  GameObject.Find("Bear(Clone)").GetComponent<Bear>().LogicalPosition;
            Assert.AreNotEqual(position, newPosition, "Restarting did not put the player back.");

        }

        private void MenuTearDown()
        {
            var menu = GameObject.Find("MainMenuObjects");
            GameObject.Destroy(menu);
        }
    }
}