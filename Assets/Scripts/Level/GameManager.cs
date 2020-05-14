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
    /// Internal flag to check how many tiles are moving (not flipping)
    /// </summary>
    private int _movingTile = 0;

    /// <summary>
    /// Internal flag to check if the board is flipping
    /// </summary>
    private bool _flipping = false;

    /// <summary>
    /// Internal flag to check if the bear is moving
    /// </summary>
    private bool _movingBear = false;

    public int MovingTile { get => _movingTile; private set => _movingTile = value;}
    public bool Flipping { get => _flipping; private set => _flipping = value;}
    public bool MovingBear { get => _movingBear; set => _movingBear = value; }

    enum Lock {Flip, Bear, Tile}

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
    /// Coroutine of moving the given objects
    /// </summary>
    /// <param name="moveDuration"></param>
    /// <param name="speed"></param>
    /// <param name="destinations"></param>
    /// <param name="transforms"></param>
    /// <param name="after"></param>
    /// <returns></returns>
    private IEnumerator MovingCoroutine(float moveDuration, float speed, Vector3[] destinations,
        Transform[] transforms, Action after, Lock lockToOpen)
    {
        var start = Time.time;

        // main loop
        while (Time.time - start < moveDuration) // if time overflows, infinite loop
        {
            // move every object
            for (int i = 0; i < transforms.Length; i++)
            {
                var movingObject = transforms[i];
                var destination = destinations[i];

                var step = speed * Time.deltaTime;
                movingObject.position = Vector3.MoveTowards(
                    movingObject.position, destination, step
                );
            }
            yield return null;
        }

        // clamp the values to the integer
        foreach (var movingObject in transforms)
        {
            var current = movingObject.position;
            current.Set(Mathf.Round(current.x), Mathf.Round(current.y), Mathf.Round(current.z));
            movingObject.position = current;
        }

        // Run the after function
        after();

        // At the end, open the right lock
        switch (lockToOpen)
        {
            case Lock.Bear:
                MovingBear = false;
                break;
            case Lock.Flip:
                Flipping = false;
                break;
            case Lock.Tile:
                MovingTile--; // decrease the counter
                break;
        }
    }

    /// <summary>
    /// Move a group of transforms according to the target positions using a coroutine
    /// </summary>
    /// <param name="moveDuration">Duration of the group movement</param>
    /// <param name="speed">Array of movement speed (unit/second)</param>
    /// <param name="destinations">Array of world coordinates to go to</param>
    /// <param name="transforms">Array of the object's transforms</param>
    /// <param name="before">Void function to do before the coroutine starts</param>
    /// <param name="after">Void function to do after the coroutine ends</param>
    /// <returns></returns>
    private Coroutine Move(float moveDuration, float speed, Vector3[] destinations, Transform[] transforms,
        Action before, Action after, Lock lockToOpen)
    {
        // check if arrays are of equal length
        if (destinations.Length != transforms.Length) 
            throw new Exception($"Array of destinations do not have the same length as array of transforms: {destinations.Length} vs. {transforms.Length}");

        // execute preliminary function
        before();

        // start coroutine when conditions are met
        return StartCoroutine(MovingCoroutine(moveDuration, speed, destinations, transforms, after, lockToOpen));
    }

    /// <summary>
    /// Lowers a tile over a period of time to the given position, respecting game movement locks
    /// </summary>
    /// <param name="moveDuration">Duration of the group movement</param>
    /// <param name="speed">Movement speed (unit/second)</param>
    /// <param name="destination">World coordinates to go to</param>
    /// <param name="tile">The object's transforms</param>
    /// <param name="before">Void function to call before the coroutine starts</param>
    /// <param name="after">Void function to call after the coroutine ends</param>
    /// <param name="forceStartWhenDenied">If starting the coroutine is denied, force it to start after the given function returns true.</param>
    /// <returns></returns>
    public Coroutine LowerTile(float moveDuration, float speed, Vector3 destination, Transform tile,
        Action before, Action after, Func<bool> forceStartWhenDenied)
    {
        // only check for flipping
        if (Flipping)
        {
            if(false == forceStartWhenDenied()) return null;
        }

        // increase the movement counter
        MovingTile++;

        return Move(moveDuration, speed, new Vector3[] {destination},
            new Transform[] {tile}, before, after, Lock.Tile);
    }

    /// <summary>
    /// Moves the player to the target position, respecting game movement locks
    /// </summary>
    /// <param name="moveDuration">Duration of the movement</param>
    /// <param name="speed">Speed of the movement</param>
    /// <param name="destination">World coordinate of the destination</param>
    /// <param name="player">Transform of the player (bear)</param>
    /// <param name="before">Void function to call before the coroutine starts</param>
    /// <param name="after">Void function to call after the coroutine ends</param>
    /// <param name="forceStartWhenDenied">If starting the coroutine is denied, force it to start after the given function returns true.</param>
    /// <returns></returns>
    public Coroutine PlayerMove(float moveDuration, float speed, Vector3 destination, Transform player,
        Action before, Action after, Func<bool> forceStartWhenDenied)
    {
        // check for all locks but moving tile
        if (Flipping || MovingBear)
        {
            if (false == forceStartWhenDenied()) return null;
        }

        MovingBear = true;

        return Move(moveDuration, speed, new Vector3[] {destination}, new Transform[] {player},
            before, after, Lock.Bear);
    }


    /// <summary>
    /// Flips the board with a coroutine
    /// </summary>
    /// <param name="flipDuration">Duration of the flip</param>
    /// <param name="speed">Speed of the flip (units/second)</param>
    /// <param name="board">The game board</param>
    /// <param name="before">Void function to call before</param>
    /// <param name="after">Void function to call after</param>
    /// <param name="forceStartWhenDenied">If starting the coroutine is denied, force it to start after the given function returns true.</param>
    /// <returns></returns>
    public Coroutine Flip(float flipDuration, float speed, Board board,
        Action before, Action after, Func<bool> forceStartWhenDenied)
    {
        // Check for all locks
        if (Flipping || MovingBear || MovingTile > 0)
        {
            if (false == forceStartWhenDenied()) return null;
        }

        Flipping = true;

        // group all flipping tiles into an array
        // group all flipping destinations into an array
        // NOTE: exclude the tile the player is on
        if (board.tileCount == 1) 
            throw new Exception("Only tile in the level, the level should be considered as lost.");

        var tiles = new Transform[board.tileCount - 1];
        var destinations = new Vector3[board.tileCount - 1];

        GameObject playerTile = null;

        int i = 0;
        foreach (var item in board.tiles)
        {
            if (item == null) continue; // no tiles at the location

            // skip player tile
            if (board.player.transform.position.x == item.transform.position.x &&
                board.player.transform.position.y == item.transform.position.y)
            {
                playerTile = item;
                continue;
            }

            tiles[i] = item.transform;
            destinations[i] = item.GetComponent<Tile>().FlipPosition();
            i++;
        }
        
        return Move(flipDuration, speed, destinations, tiles, before,
            
            () => {
                // change the tile states of each tile
                foreach (var item in tiles)
                {
                    item.GetComponent<Tile>().Flip();
                }

                after();
            },
            Lock.Flip);
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
    /// List of messages to display when an orb is collected
    /// </summary>
    public string[] Messages;

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

        // Verify the messages array
        if (Messages == null) throw new System.ArgumentException("No messages field is given");
        if (Messages.Length > Orbs.Length)
            throw new System.ArgumentException($"There are more messages than orbs: {Messages.Length} > {Orbs.Length}");

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

