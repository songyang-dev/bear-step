using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMenuUI : MonoBehaviour
{

    public GameObject GameManager;

    private GameObject _winMenu;

    private GameObject _nextLevelButton;

    private void Start()
    {
        GameManager gm = GameManager.GetComponent<GameManager>();
        _winMenu = gm.WinMenuUI;
        _nextLevelButton = gm.NextLevelButton;

        ChangeNextLevelText();
    }

    private void ChangeNextLevelText()
    {
        // Change the next level button text if it is the last level
        PlayerProgress currentPlayer = PlayerProgress.CurrentPlayer;
        if (currentPlayer == null) return;

        if (currentPlayer.GetNextLevelName() == null)
        {
            var text = _nextLevelButton.GetComponentInChildren<Text>();
            text.text = "Levels Completed";
        }
    }

    public void Show()
    {
        _winMenu.SetActive(true);
    }

    public void NextLevel()
    {
        var player = PlayerProgress.CurrentPlayer;
        player.MoveToNextLevel();
    }
}
