using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public GameObject Main;

    public GameObject SaveFileSelect;

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
