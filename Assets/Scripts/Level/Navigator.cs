using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Handles the movement of objects in the scene
/// </summary>
public class Navigator
{
    private GameManager _gameManager;

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

    /// <summary>
    /// Initializes a navigator for the scene
    /// </summary>
    /// <param name="gameManager">Game manager of the scene</param>
    public Navigator(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    /// <summary>
    /// How many tiles are currently moving
    /// </summary>
    /// <value></value>
    public int MovingTile { get => _movingTile; private set => _movingTile = value;}

    /// <summary>
    /// Whether the board is moving
    /// </summary>
    /// <value></value>
    public bool Flipping { get => _flipping; private set => _flipping = value;}

    /// <summary>
    /// Whether the bear is moving
    /// </summary>
    /// <value></value>
    public bool MovingBear { get => _movingBear; private set => _movingBear = value; }

    /// <summary>
    /// Enum of flags telling which flag to release when a movement is done
    /// </summary>
    enum Lock {Flip, Bear, Tile}

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
        return _gameManager.StartCoroutine(MovingCoroutine(moveDuration, speed, destinations, transforms, after, lockToOpen));
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