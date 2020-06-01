using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenuUI : MonoBehaviour
{

    public GameObject gameManager;

    private GameObject winMenu;

    private void Start()
    {
        winMenu = gameManager.GetComponent<GameManager>().winMenuUI;
    }

    public void Show()
    {
        winMenu.SetActive(true);
    }
}
