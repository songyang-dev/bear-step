using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public GameObject Main;

    public GameObject SaveFileSelect;
    
    public GameObject NewSaveFile;
    
    public GameObject SaveFilesExisting;
    
    public InputField NewSaveName;

    public void PlayButton()
    {
        Main.SetActive(false);
        SaveFileSelect.SetActive(true);
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void OptionsButton()
    {

    }

    public void AboutButton()
    {

    }

    public void SaveSelectBackButton()
    {
        Main.SetActive(true);
        SaveFileSelect.SetActive(false);
    }

}
