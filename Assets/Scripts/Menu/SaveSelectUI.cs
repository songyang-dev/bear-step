using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Script related to the presentation of save files
/// </summary>
public class SaveSelectUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the main menu game object
    /// </summary>
    public GameObject MainMenu;

    /// <summary>
    /// Reference to the new save file screen game object
    /// </summary>
    private GameObject newSaveFile;

    /// <summary>
    /// Reference to the screen presenting the save files
    /// </summary>
    private GameObject saveFilesExisting;

    /// <summary>
    /// Reference to the input field for typing a new save file
    /// </summary>
    private InputField newSaveName;

    /// <summary>
    /// Name of the save file in slot 1
    /// </summary>
    private string save1;

    /// <summary>
    /// Name of the save file in slot 2
    /// </summary>
    private string save2;

    /// <summary>
    /// Name of the save file in slot 3
    /// </summary>
    private string save3;

    /// <summary>
    /// Number of the selected save slot
    /// </summary>
    private int editSaveSlot;

    /// <summary>
    /// Loads all save slots and presents on the screen
    /// </summary>
    private void Start()
    {
        // Find existing save files
        // If not found, string is ""
        save1 = PlayerPrefs.GetString("save1");
        save2 = PlayerPrefs.GetString("save2");
        save3 = PlayerPrefs.GetString("save3");

        PresentSaveFileButton(1, save1);
        PresentSaveFileButton(2, save2);
        PresentSaveFileButton(3, save3);

        var mainMenu = MainMenu.GetComponent<MainMenuUI>();
        newSaveFile = mainMenu.NewSaveFile;
        saveFilesExisting = mainMenu.SaveFilesExisting;
        newSaveName = mainMenu.NewSaveName;
    }

    /// <summary>
    /// Configures the display of a save file button
    /// </summary>
    /// <param name="buttonNumber"></param>
    /// <param name="saveName"></param>
    void PresentSaveFileButton(int buttonNumber, string saveName)
    {
        var button = transform.Find($"Save Files").gameObject
            .transform.Find($"Save {buttonNumber}").gameObject;
        var text = button.GetComponentInChildren<Text>();

        // no existing save file for this button
        if (saveName.Equals(""))
        {
            text.text = "New Save";
        }
        // there is an existing save file
        else
        {
            text.text = saveName;
        }
    }

    /// <summary>
    /// Listener to save button 1
    /// </summary>
    public void SaveFile1()
    {
        if (save1.Equals("")) CreateNewSave(1);
        else StartLevel(save1);
    }

    /// <summary>
    /// Listener to save button 2
    /// </summary>
    public void SaveFile2()
    {
        if (save2.Equals("")) CreateNewSave(2);
        else StartLevel(save2);
    }

    /// <summary>
    /// Listener to save button 3
    /// </summary>
    public void SaveFile3()
    {
        if (save3.Equals("")) CreateNewSave(3);
        else StartLevel(save3);
    }

    /// <summary>
    /// Changes to the screen for creating new save files
    /// </summary>
    /// <param name="slotNumber"></param>
    private void CreateNewSave(int slotNumber)
    {
        // change the ui to the new save file prompt
        newSaveFile.SetActive(true);
        saveFilesExisting.SetActive(false);

        editSaveSlot = slotNumber;
    }

    /// <summary>
    /// User clicks the OK button next to the input field.
    /// Sets up the player save file and begins the level.
    /// </summary>
    public void OkButton()
    {
        var input = newSaveName.text;

        // checks if the name is inappropriate
        switch (input)
        {
            case "New Save":
            case "new save":
            case "new Save":
            case "New save":
                PromptInvalidName();
                break;
        }

        StartLevel(input);
    }

    /// <summary>
    /// Sets up the player progress class and links the save slot.
    /// Then starts the level.
    /// </summary>
    /// <param name="name"></param>
    private void StartLevel(string name)
    {
        // initiates a player progress
        new PlayerProgress(name);

        // attach this to the save slot
        var slot = $"save{editSaveSlot}";
        PlayerPrefs.SetString(slot, name);

        // Change scene

        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Should display an error message on the menu about invalid name
    /// </summary>
    private void PromptInvalidName()
    {
        throw new NotImplementedException();
    }
}
