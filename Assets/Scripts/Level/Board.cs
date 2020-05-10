using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary>
/// Cardinal direction in the game
/// </summary>
public enum Direction
{
    North,
    South,
    East,
    West
}

public class Board : MonoBehaviour
{
    
    public GameObject gameManager;

    /// <summary>
    /// Width x Height
    /// </summary>
    public int[] boardDimension;

    /// <summary>
    /// 2d array of tile gameobjects, [0,0] is top left corner
    /// </summary>
    public GameObject[,] tiles;

    /// <summary>
    /// List of orb gameobjects
    /// </summary>
    public GameObject[] orbs;

    /// <summary>
    /// List of board character, including the player, as gameobjects
    /// </summary>
    public GameObject[] characters;

    /// <summary>
    /// Tile prefab asset
    /// </summary>
    public GameObject tilePrefab;

    /// <summary>
    /// Orb prefab asset
    /// </summary>
    public GameObject orbPrefab;

    /// <summary>
    /// Bear prefab asset
    /// </summary>
    public GameObject bearPrefab;

    /// <summary>
    /// One of the playable character in the level
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Event of the player touching an orb
    /// </summary>
    public UnityEvent touchOrb;

    public Dictionary<Direction, Vector2Int> logicalDirections = new Dictionary<Direction, Vector2Int>
    {
        {Direction.North, new Vector2Int(0,-1)},
        {Direction.South, new Vector2Int(0,1)},
        {Direction.East, new Vector2Int(1,0)},
        {Direction.West, new Vector2Int(-1,0)}
    };

    /// <summary>
    /// Destroys the game object loaded by Level(str)
    /// </summary>
    public void TearDown()
    {
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile);
        }

        foreach (var orb in orbs)
        {
            DestroyImmediate(orb);
        }

        foreach (var character in characters)
        {
            DestroyImmediate(character);
        }
    }

    /// <summary>
    /// Sets up the board on the x,y plane
    /// </summary>
    /// <param name="json">Json data of the level</param>
    public void SetUp(JSONLevel json)
    {
        SetUpTiles(json);

        SetUpOrbs(json);

        SetUpCharacters(json);
    }

    /// <summary>
    /// Sets up the tiles of the level
    /// </summary>
    /// <param name="json">Json data of the level</param>
    private void SetUpTiles(JSONLevel json)
    {
        boardDimension = json.Dimension;

        // Place the tiles
        tiles = new GameObject[boardDimension[0], boardDimension[1]];

        // Loop through the flattened array
        for (int i = 0; i < boardDimension[0] * boardDimension[1]; i++)
        {

            // from flat 1-d to 2-d
            int x = i % boardDimension[0];
            int y = i / boardDimension[0];

            // if 0, leave it null
            if (json.Tiles[i] == 0)
            {
                tiles[x, y] = null;
                continue;
            }

            // otherwise, instantiate a tile
            var newTile =
                GameObject.Instantiate(tilePrefab, new Vector3(x, boardDimension[1] - y - 1, 0),
                    Quaternion.Euler(-90, 0, 0), this.transform);

            // set its state
            switch (json.Tiles[i])
            {
                case 1:
                    newTile.GetComponent<Tile>().InitiateTileState(TileState.Down);
                    break;
                case 2:
                    newTile.GetComponent<Tile>().InitiateTileState(TileState.Up);
                    break;
                default:
                    throw new System.ArgumentException($"Invalid tile state, seen {json.Tiles[i]}");
            }

            tiles[x, y] = newTile;
        }
    }

    /// <summary>
    /// Sets up the orbs of the level
    /// </summary>
    /// <param name="json">Serialized json class of the level</param>
    private void SetUpOrbs(JSONLevel json)
    {
        boardDimension = json.Dimension;

        // Place the orbs
        orbs = new GameObject[json.Orbs.Length];
        for (int i = 0; i < orbs.Length; i++)
        {
            orbs[i] = GameObject.Instantiate(orbPrefab,
                new Vector3(json.Orbs[i].Coord[0], boardDimension[1] - json.Orbs[i].Coord[1] - 1, -1),
                Quaternion.Euler(-90, 0, 0), this.transform);
        }
    }

    /// <summary>
    /// Sets up the characters of the level, just the bear for now
    /// </summary>
    /// <param name="json">Serialized json class of the level</param>
    private void SetUpCharacters(JSONLevel json)
    {
        boardDimension = json.Dimension;

        // Set up characters
        characters = new GameObject[json.Start.Length];

        // only one bear allowed
        player = null;

        for (int i = 0; i < json.Start.Length; i++)
        {
            if (json.Start[i].Name.Equals("bear"))
            {
                if (player != null) throw new System.ArgumentException("More than one bear in the level description");

                player = GameObject.Instantiate(bearPrefab,
                    new Vector3(json.Start[i].Coord[0], boardDimension[1] - json.Start[i].Coord[1] - 1, 0),
                    Quaternion.Euler(-90, 0, 0),
                    this.transform);

                characters[i] = player;
            }
        }
    }

    /// <summary>
    /// Listener to the event of moving the player
    /// </summary>
    /// <param name="context">Unity input system context</param>
    public void MoveAllPlayers(InputAction.CallbackContext context)
    {
        foreach (var player in characters)
        {
            if (player.name.Contains("Bear"))
                player.GetComponent<Bear>().Move(context);
        }
    }

    /// <summary>
    /// After the translation coroutine of the player is done, its positions are
    /// used to check for collision with orbs.
    /// </summary>
    /// <returns>Whether an orb was touched</returns>
    public bool PlayerMoved()
    {
        var location = player.transform.position;

        GameObject contact = null;

        foreach (var orb in orbs)
        {
            if (orb == null) continue;
            if (orb.transform.position.x == location.x && orb.transform.position.y == location.y)
            {
                // contact made between player and orb
                contact = orb;
                // only one contact possible
                break;
            }
        }

        // no contact case
        if (contact == null) return false;

        // there is a contact orb, trigger event
        touchOrb.Invoke();

        contact.SetActive(false); // deactivates the orb

        return true;
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    public void OrbTouchDebug()
    {
        Debug.Log("Touched an orb");
    }

    /// <summary>
    /// Checks if the given move is legal according to the game state
    /// </summary>
    /// <param name="logicalCoordinate">Logical position</param>
    /// <param name="to">Direction to move to</param>
    /// <returns>Whether the move is legal</returns>
    public bool IsLegalMove(Vector2Int logicalCoordinate, Direction to)
    {
        VerifyLogicalCoordinate(logicalCoordinate, true);

        var destination = logicalCoordinate + logicalDirections[to];

        // destination is valid in the game
        if (VerifyLogicalCoordinate(destination, false))
        {
            // check if the tile is at the same level as the logicalCoordinate
            var fromState = tiles[logicalCoordinate[0], logicalCoordinate[1]]
                .GetComponent<Tile>().State;
            var toState = tiles[destination[0], destination[1]]
                .GetComponent<Tile>().State;
            if (fromState == toState) return true;
            else return false;
        }
        else return false;
    }


    /// <summary>
    /// Verifies if the logical coordinate is valid in the game
    /// </summary>
    /// <param name="coord">Logical coordinate</param>
    /// <param name="throwExceptions">Whether to throw exceptions instead</param>
    /// <returns>Whether the given move is legal or not</returns>
    private bool VerifyLogicalCoordinate(Vector2Int coord, bool throwExceptions)
    {
        // check game bounds
        if (coord.x < 0 || coord.x >= boardDimension[0])
        {
            if (throwExceptions) throw new System.Exception($"x-component of the logical coordinate is out of bounds: {coord.x}");
            else return false;
        }

        if (coord.y < 0 || coord.y >= boardDimension[1])
        {
            if (throwExceptions) throw new System.Exception($"y-component of the logical coordinate is out of bounds: {coord.y}");
            else return false;
        }

        // no tiles at the current position
        if (null == tiles[coord[0], coord[1]])
        {
            if (throwExceptions) throw new System.Exception($"Current logical position is on an empty tile {coord}");
            else return false;
        }
        return true;
    }

    /// <summary>
    /// Lowers the tile that the player just moved out of
    /// </summary>
    /// <param name="performedMoveDirection"></param>
    public void LowerPreviousTile(Direction performedMoveDirection)
    {
        var previousLogicalPosition = player.GetComponent<Bear>().logicalPosition
            - logicalDirections[performedMoveDirection];
        
        tiles[previousLogicalPosition[0], previousLogicalPosition[1]].GetComponent<Tile>()
            .ChangeTileState(TileState.Down);
    }
}
