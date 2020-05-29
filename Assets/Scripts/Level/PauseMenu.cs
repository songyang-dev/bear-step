using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Script attached to the pause menu UI, contains listeners to UI buttons
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>
    /// Whether the game is paused
    /// </summary>
    public bool GameIsPaused = false;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public GameObject gameManager;

    /// <summary>
    /// Reference to the pause menu panel
    /// </summary>
    private GameObject pauseMenuUI;

    /// <summary>
    /// Name of the level scene
    /// </summary>
    private string levelScene;

    /// <summary>
    /// Name of the menu scene
    /// </summary>
    private string menuScene;

    /// <summary>
    /// Link references
    /// </summary>
    private void Start()
    {
        var gm = gameManager.GetComponent<GameManager>();
        this.pauseMenuUI = gm.pauseMenuUI;
        this.levelScene = gm.levelScene;
        this.menuScene = gm.menuScene;
    }

    /// <summary>
    /// Pauses the game and freezes time
    /// </summary>
    public void Pause()
    {
        this.gameManager.GetComponent<PlayerInput>().SwitchCurrentActionMap("Paused");

        this.GameIsPaused = true;
        Time.timeScale = 0;

        pauseMenuUI.SetActive(true);
    }

    /// <summary>
    /// Resumes the game
    /// </summary>
    public void Resume()
    {
        this.gameManager.GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");

        this.GameIsPaused = false;
        Time.timeScale = 1;

        pauseMenuUI.SetActive(false);
    }

    /// <summary>
    /// Restarts the level
    /// </summary>
    public void Restart()
    {
        this.GameIsPaused = false;
        Time.timeScale = 1;

        SceneManager.LoadScene(levelScene);
    }

    /// <summary>
    /// Go to the main menu
    /// </summary>
    public void Menu()
    {
        SceneManager.LoadScene(menuScene);
    }
}
