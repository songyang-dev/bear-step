using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script related to the presentation of save files
/// </summary>
public class SaveSelectUI : MonoBehaviour
{
    private string save1;
    private string save2;
    private string save3;

    private void Start()
    {
        // Find existing save files
        save1 = PlayerPrefs.GetString("save1");
        save2 = PlayerPrefs.GetString("save2");
        save3 = PlayerPrefs.GetString("save3");

        PresentSaveFileButton(1, save1);
        PresentSaveFileButton(2, save2);
        PresentSaveFileButton(3, save3);
    }

    /// <summary>
    /// Configures the display of a save file button
    /// </summary>
    /// <param name="buttonNumber"></param>
    /// <param name="saveName"></param>
    void PresentSaveFileButton(int buttonNumber, string saveName)
    {
        var button = transform.Find($"Save {buttonNumber}").gameObject;
        var text = button.GetComponentInChildren<Text>();

        // no existing save file for this button
        if (saveName.Equals(""))
        {
            text.text = "";
        }
        // there is an existing save file
        else
        {
            text.text = saveName;
        }
    }

    public void SaveFile1()
    {

    }

    public void SaveFile2()
    {
        
    }

    public void SaveFile3()
    {
        
    }
}
