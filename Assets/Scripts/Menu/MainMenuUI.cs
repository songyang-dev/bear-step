using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the main menu screen with the play button
    /// </summary>
    public GameObject Main;

    /// <summary>
    /// Reference to the save file selection screen
    /// </summary>
    public GameObject SaveFileSelect;
    
    /// <summary>
    /// Reference to the save file creation screen
    /// </summary>
    public GameObject NewSaveFile;
    
    /// <summary>
    /// Reference to the save file slots section of the save selection screen
    /// </summary>
    public GameObject SaveFilesExisting;
    
    /// <summary>
    /// Reference to the input field for creating a new save file
    /// </summary>
    public InputField NewSaveName;

    /// <summary>
    /// Reference to the prompt for confirming deleting a save file
    /// </summary>
    public GameObject ConfirmDeletePrompt;
    
    /// <summary>
    /// Reference to the display when an invalid name is entered
    /// </summary>
    public GameObject InvalidNameDisplay;

    /// <summary>
    /// Clicks the play button on the menu
    /// </summary>
    public void PlayButton()
    {
        Main.SetActive(false);
        SaveFileSelect.SetActive(true);
    }

    /// <summary>
    /// Clicks the exit button on the menu
    /// </summary>
    public void ExitButton()
    {
        Application.Quit();
    }

    /// <summary>
    /// Clicks the options button on the menu
    /// </summary>
    public void OptionsButton()
    {

    }

    /// <summary>
    /// Clicks the about button on the menu
    /// </summary>
    public void AboutButton()
    {

    }

    /// <summary>
    /// Clicks the back button when choosing save files
    /// </summary>
    public void SaveSelectBackButton()
    {
        Main.SetActive(true);
        SaveFileSelect.SetActive(false);
    }

}
