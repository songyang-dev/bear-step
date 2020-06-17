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
    public string levelScene;

    /// <summary>
    /// Name of the menu scene
    /// </summary>
    public string menuScene;

    /// <summary>
    /// Reference to the scene's board
    /// </summary>
    public GameObject board;

    /// <summary>
    /// Reference to the scene's camera
    /// </summary>
    public GameObject cam;

    /// <summary>
    /// Whether the scene is running as a test
    /// </summary>
    public bool test;

    /// <summary>
    /// Path to the test level
    /// </summary>
    public string testLevel;
    
    /// <summary>
    /// Navigator module of the scene
    /// </summary>
    public Navigator navigator;
    
    /// <summary>
    /// Prefab of the bear
    /// </summary>
    public GameObject bearPrefab;

    /// <summary>
    /// Prefab of the orb
    /// </summary>
    public GameObject orbPrefab;

    /// <summary>
    /// Prefab of the tile
    /// </summary>
    public GameObject tilePrefab;

    /// <summary>
    /// Reference to the orb message animator
    /// </summary>
    public Animator orbMessageUIAnimator;

    /// <summary>
    /// Reference to the orb count UI
    /// </summary>
    public GameObject orbCountUI;

    /// <summary>
    /// Reference to the orb message UI
    /// </summary>
    public GameObject orbMessageUI;
    
    /// <summary>
    /// Reference to the panel of the pause menu canvas
    /// </summary>
    public GameObject pauseMenuUI;
    
    /// <summary>
    /// Reference to the panel of the win menu canvas
    /// </summary>
    public GameObject winMenuUI;

    /// <summary>
    /// Reference to the button of "Next Level"
    /// </summary>
    public GameObject nextLevelButton;

    /// <summary>
    /// Event of winning the level
    /// </summary>
    public UnityEvent winLevel;
    

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

        if (test) {
            new PlayerProgress("test");
            Level(testLevel);
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
        board.GetComponent<Board>().SetUp(json);
    }

    /// <summary>
    /// Sets up where the camera starts and its limits
    /// </summary>
    private void SetUpCamera()
    {
        cam.GetComponent<CameraController>().SetUp();
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
        board.GetComponent<Board>().TearDown();
    }
}
