using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;

/// <summary>
/// Script attached to an empty and handles player inputs. Also provides functions
/// to move game objects in the scene, such as tiles and the player.
/// </summary>
public class GameManager : MonoBehaviour
{   
    /// <summary>
    /// Name of the level scene
    /// </summary>
    public string LevelScene;

    /// <summary>
    /// Name of the menu scene
    /// </summary>
    public string MenuScene;

    /// <summary>
    /// Reference to the scene's board
    /// </summary>
    public GameObject Board;

    /// <summary>
    /// Reference to the scene's camera
    /// </summary>
    public GameObject Cam;

    /// <summary>
    /// Whether the scene is running as a test
    /// </summary>
    public bool Test;

    /// <summary>
    /// Path to the test level
    /// </summary>
    public string TestLevel;
    
    /// <summary>
    /// Navigator module of the scene
    /// </summary>
    public Navigator navigator;
    
    /// <summary>
    /// Prefab of the bear
    /// </summary>
    public GameObject BearPrefab;

    /// <summary>
    /// Prefab of the orb
    /// </summary>
    public GameObject OrbPrefab;

    /// <summary>
    /// Prefab of the tile
    /// </summary>
    public GameObject TilePrefab;

    /// <summary>
    /// Reference to the orb message animator
    /// </summary>
    public Animator OrbMessageUIAnimator;

    /// <summary>
    /// Reference to the orb count UI
    /// </summary>
    public GameObject OrbCountUI;

    /// <summary>
    /// Reference to the orb message UI
    /// </summary>
    public GameObject OrbMessageUI;
    
    /// <summary>
    /// Reference to the panel of the pause menu canvas
    /// </summary>
    public GameObject PauseMenuUI;
    
    /// <summary>
    /// Reference to the panel of the win menu canvas
    /// </summary>
    public GameObject WinMenuUI;

    /// <summary>
    /// Reference to the button of "Next Level"
    /// </summary>
    public GameObject NextLevelButton;

    /// <summary>
    /// Event of winning the level
    /// </summary>
    public UnityEvent WinLevel;
    

    /// <summary>
    /// Enum of flags telling which flag to release when a movement is done
    /// </summary>
    enum Lock {Flip, Bear, Tile}

    /// <summary>
    /// Initiates the level
    /// </summary>
    void Start()
    {
        navigator = new Navigator(this);

        if (Test) {
            new PlayerProgress("test");
            Level(TestLevel);
        }
        else {
            Level(PlayerProgress.CurrentPlayer.GetCurrentLevelName());
        }
    }

    /// <summary>
    /// Sets up the board of the level
    /// </summary>
    /// <param name="json">Serialized json class of the level</param>
    private void SetUpBoard(JSONLevel json)
    {
        Board.GetComponent<Board>().SetUp(json);
    }

    /// <summary>
    /// Sets up where the camera starts and its limits
    /// </summary>
    private void SetUpCamera()
    {
        Cam.GetComponent<CameraController>().SetUp();
    }

    /// <summary>
    /// Builds a level from the given file path
    /// </summary>
    /// <param name="name">Path of the JSON level description</param>
    /// <param name="gameManager">The level's scene game manager</param>
    private void Level(string name)
    {
        // Read json
        var jsonTextFile = Resources.Load<TextAsset>(name);
        var json = JsonUtility.FromJson<JSONLevel>(jsonTextFile.text);
        json.Verify();

        // Start building the level board
        SetUpBoard(json);
        SetUpCamera();
    }

    /// <summary>
    /// Destroys the level's board
    /// </summary>
    public void TearDown()
    {
        Board.GetComponent<Board>().TearDown();
    }
}
