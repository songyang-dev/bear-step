using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class MenuButtons : MenuSceneSetUp
    {
        
        /// <summary>
        /// Tests the play, options and quit buttons when the scene starts
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator FirstMenu()
        {
            var main = root.transform.Find("Main Menu").Find("Main Menu UI").Find("Main").gameObject;

            Assert.True(main.activeInHierarchy, "Main object is not active");

            var play = main.transform.Find("Play Button").gameObject;
            Assert.True(play.activeInHierarchy, "Play button is not active");

            var options = main.transform.Find("Options Button").gameObject;
            Assert.True(options.activeInHierarchy, "Options button is not active");

            var exit = main.transform.Find("Exit Button").gameObject;
            Assert.True(exit.activeInHierarchy, "Exit button is not active");

            var about = main.transform.Find("About Button").gameObject;
            Assert.True(about.activeInHierarchy, "About button is not active");

            yield return null;
        }

        [UnityTest]
        public IEnumerator PlayButton()
        {
            var menuScript = root.GetComponentInChildren<MainMenuUI>();

            // click play
            menuScript.PlayButton();

            var main = root.transform.Find("Main Menu").Find("Main Menu UI").Find("Main").gameObject;
            Assert.False(main.activeInHierarchy, "Main menu is still active");

            var saveSelect = root.transform.Find("Main Menu").Find("Main Menu UI").Find("SaveFileSelect").gameObject;
            Assert.True(saveSelect.activeInHierarchy, "Save selection screen is not active");

            var back = saveSelect.transform.Find("Back Button").gameObject;
            Assert.True(back.activeInHierarchy, "Back button is not active");

            var saveFiles = saveSelect.transform.Find("Save Files").gameObject;
            Assert.True(saveFiles.activeInHierarchy, "Save file slots are not presented");

            yield return null;
        }

        [UnityTest]
        public IEnumerator NewSave()
        {
            var menuScript = root.GetComponentInChildren<MainMenuUI>();

            // click play
            menuScript.PlayButton();

            var saveFileSelect = GameObject.Find("SaveFileSelect");
            Assert.True(saveFileSelect.activeInHierarchy, "Save file selection is not active");
            
            var save1 = GameObject.Find("Save 1");
            Assert.True(save1.activeInHierarchy, "Save 1 button is not active");

            var save1Delete = save1.transform.Find("Delete Button").gameObject;
            Assert.False(save1Delete.activeInHierarchy, "Save 1 delete button is active");
            
            // click save 1
            yield return null;
            save1.GetComponent<Button>().onClick.Invoke();

            var saveFiles = saveFileSelect.transform.Find("Save Files").gameObject;
            Assert.False(saveFiles.activeInHierarchy, "Save file slot selection is still active");

            var newSaveFile = saveFileSelect.transform.Find("New Save File").gameObject;
            Assert.True(newSaveFile.activeInHierarchy, "New save file is not active");

            var invalidPrompt = newSaveFile.transform.Find("Invalid Name Prompt").gameObject;
            Assert.False(invalidPrompt.activeInHierarchy, "Invalid prompt is active");

            yield return null;
        }

        [UnityTest, Order(1)]
        public IEnumerator CreateSave()
        {
            var menuScript = root.GetComponentInChildren<MainMenuUI>();

            // click play
            menuScript.PlayButton();
            
            yield return null;

            // clean up save 3
            var save3 = GameObject.Find("Save 3");
            Transform transform = save3.transform.Find("Delete Button");
            if (transform.gameObject.activeInHierarchy)
                transform.GetComponent<Button>().onClick.Invoke();

            // click save 3
            save3.GetComponent<Button>().onClick.Invoke();

            yield return null;

            // enter a save name
            var newSave = GameObject.Find("New Save File");
            newSave.transform.Find("InputField").GetComponent<InputField>().text = "test name";
            newSave.transform.Find("Ok Button").GetComponent<Button>().onClick.Invoke();

            GameObject.Destroy(root);

            yield return null;

            // now in level scene
            Assert.AreEqual("Level", SceneManager.GetActiveScene().name, "Scene has not changed to Level");
            GameObject.Find("Game UI").GetComponentInChildren<Button>().onClick.Invoke();
            //GameObject.Find("Pause Menu UI").transform.Find("Quit Button").GetComponent<Button>().onClick.Invoke();

            root = GameObject.Find("LevelGameObjects");
        }

        [UnityTest, Order(2)]
        public IEnumerator DeleteSave()
        {
            // remove the save
            Button play = GameObject.Find("Play Button").GetComponent<Button>();
            Assert.True(play.gameObject.activeInHierarchy);
            play.onClick.Invoke();
            yield return null;

            var save3 = GameObject.Find("Save 3");

            var delete = save3.transform.Find("Delete Button").gameObject;
            Assert.True(delete.activeInHierarchy, "Delete button is not active");
            delete.GetComponent<Button>().onClick.Invoke();
            
            yield return null;

            // check if deleted properly
            Assert.False(delete.activeInHierarchy, "Delete button is active after deleting");
            Assert.AreEqual("New Save", save3.transform.Find("Text").GetComponent<Text>().text, "Save file name has not reset");

            yield return null;
        }
    }
}
