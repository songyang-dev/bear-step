using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{   
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
    /// Initiates the level
    /// </summary>
    void Start()
    {
        if (test) {
            Level(testLevel);
        }
        else {
            Level(PlayerPrefs.GetString("level"));
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
    /// <param name="json">Serialized json class of the level</param>
    private void SetUpCamera(JSONLevel json)
    {
        cam.GetComponent<CameraController>().SetUp(json);
    }

    /// <summary>
    /// Builds a level from the given file path
    /// </summary>
    /// <param name="name">Path of the JSON level description</param>
    /// <param name="gameManager">The level's scene game manager</param>
    public void Level(string name)
    {
        // Read json
        var jsonTextFile = Resources.Load<TextAsset>(name);
        var json = JsonUtility.FromJson<JSONLevel>(jsonTextFile.text);
        json.Verify();

        // Start building the level board
        SetUpBoard(json);
        SetUpCamera(json);
    }

    /// <summary>
    /// Destroys the level's board
    /// </summary>
    public void TearDown()
    {
        board.GetComponent<Board>().TearDown();
    }

    /// <summary>
    /// Coroutine of moving the an object linearly to a destination 
    /// </summary>
    /// <param name="moveDuration">Seconds the move lasts</param>
    /// <param name="moveDistance">Units to move</param>
    /// <param name="destination">World coordinate of the destination</param>
    /// <param name="movingObject">Transform of the object to be moved</param>
    /// <param name="afterMoving">Anything to do after moving</param>
    /// <returns>Enumerator coroutine</returns>
    private IEnumerator MovingCoroutine(float moveDuration, float moveDistance, Vector3 destination, Transform movingObject,
        Action afterMoving)
    {
        var speed = moveDistance / moveDuration;
        var start = Time.time;
        // loop for one move duration
        while (Time.time - start < moveDuration)
        {

            var step = speed * Time.deltaTime;
            movingObject.position = Vector3.MoveTowards(
                movingObject.position, destination, step
            );
            yield return null;
        }

        // clamp the values to the integer
        var current = movingObject.position;
        current.Set(Mathf.Round(current.x), Mathf.Round(current.y), Mathf.Round(current.z));
        movingObject.position = current;

        afterMoving();
    }

    /// <summary>
    /// Coroutine of moving the an object linearly to a destination 
    /// </summary>
    /// <param name="moveDuration">Seconds the move lasts</param>
    /// <param name="moveDistance">Units to move</param>
    /// <param name="destination">World coordinate of the destination</param>
    /// <param name="movingObject">Transform of the object to be moved</param>
    /// <param name="afterMoving">Anything to do after moving</param>
    /// <returns>Enumerator coroutine</returns>
    public Coroutine Move(float moveDuration, float moveDistance, Vector3 destination, Transform movingObject,
        Action afterMoving)
    {
        return StartCoroutine(MovingCoroutine(moveDuration, moveDistance, destination, movingObject,
            afterMoving));
    }
}

/// <summary>
/// JSON representation of a level
/// </summary>
[System.Serializable]
public class JSONLevel
{

    /// <summary>
    /// Name of the board, used to reference neighbors
    /// </summary>
    public string Name;

    /// <summary>
    /// Length 2 array describing the width x height of the board
    /// </summary>
    public int[] Dimension;

    /// <summary>
    /// Flattened 2d matrix of the tiles,
    /// 0 (empty tile), 1 (low tile), 2 (high tile)
    /// </summary>
    public int[] Tiles;

    /// <summary>
    /// List of locations of the orbs
    /// </summary>
    public JSONOrb[] Orbs;

    /// <summary>
    /// Starting position of the board, if any
    /// </summary>
    public JSONStart[] Start;

    /// <summary>
    /// Verifies if the board is correct
    /// </summary>
    public void Verify()
    {

        if (Name == null) throw new System.ArgumentException("Level has no name");

        // Verify the dimension property
        if (Dimension == null)
            throw new System.ArgumentException("No dimension given");
        if (Dimension.Length != 2)
            throw new System.ArgumentException("Dimension array has length " + Dimension.Length);

        if (Dimension[0] <= 0 || Dimension[1] <= 0)
            throw new System.ArgumentException("Negative dimensions");

        if (Dimension[0] * Dimension[1] != Tiles.Length)
            throw new System.ArgumentException($"Dimensions ({Dimension[0]} by {Dimension[1]}) do not match Tiles array length of {Tiles.Length}");

        // Verify the tiles array
        if (Tiles == null) throw new System.ArgumentException("Tiles is null (not given)");

        if (Tiles.Length == 0)
            throw new System.ArgumentException("Empty Tiles array");

        foreach (var item in Tiles)
        {
            if (item < 0 || item > 2)
                throw new System.ArgumentException($"Tile integer is {item} but must be 0, 1 or 2");
        }

        // Verify the orb locations
        if (Orbs != null)
        {
            foreach (var item in Orbs)
            {
                item.Verify(Dimension);
            }
        }

        // Verify start locations
        if (Start == null) throw new System.ArgumentException("No start positions given");
        foreach (var item in Start)
        {
            item.Verify(Dimension);
        }

    }
}
[System.Serializable]
public class JSONOrb
{
    public int[] Coord;

    public void Verify(int[] dimension)
    {
        if (Coord == null) return;

        if (Coord.Length != 2)
            throw new System.ArgumentException("Orb coordinate array does not have length 2, but " + Coord.Length);

        if (Coord[0] < 0 || Coord[1] < 0)
            throw new System.ArgumentException("Orb coords are negative");

        if (Coord[0] >= dimension[0] || Coord[1] >= dimension[1])
            throw new System.ArgumentException("Orb coords are out of bounds");
    }
}

[System.Serializable]
public class JSONStart
{
    public string Name;

    public int[] Coord;

    public void Verify(int[] dimension)
    {
        if (Name == null) throw new System.ArgumentException("Character name not given");

        if (Coord == null) throw new System.ArgumentException("Starting position is not given");

        if (Coord.Length != 2)
            throw new System.ArgumentException("Start position has length " + Coord.Length);

        if (Coord[0] < 0 || Coord[1] < 0)
            throw new System.ArgumentException("Negative starting coords");

        if (Coord[0] >= dimension[0] || Coord[1] >= dimension[1])
            throw new System.ArgumentException("Starting coords is out of bounds");
    }
}

