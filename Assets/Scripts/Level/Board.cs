using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

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

/// <summary>
/// Script attached to the board, handles board creation and game logic
/// </summary>
public class Board : MonoBehaviour
{
    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public GameObject GameManager;

    /// <summary>
    /// Width x Height
    /// </summary>
    [System.NonSerialized]
    public int[] boardDimension;

    /// <summary>
    /// 2d array of tile gameobjects, [0,0] is top left corner
    /// </summary>
    [System.NonSerialized]
    public GameObject[,] tiles;

    /// <summary>
    /// How many active tiles are there?
    /// </summary>
    [System.NonSerialized]
    public int tileCount = 0;

    /// <summary>
    /// List of orb gameobjects
    /// </summary>
    [System.NonSerialized]
    public GameObject[] orbs;

    /// <summary>
    /// List of board character, including the player, as gameobjects
    /// </summary>
    [System.NonSerialized]
    public GameObject[] characters;

    /// <summary>
    /// Tile prefab asset
    /// </summary>
    private GameObject tilePrefab;

    /// <summary>
    /// Orb prefab asset
    /// </summary>
    private GameObject orbPrefab;

    /// <summary>
    /// Bear prefab asset
    /// </summary>
    private GameObject bearPrefab;

    /// <summary>
    /// One of the playable character in the level
    /// </summary>
    [System.NonSerialized]
    public GameObject player;

    /// <summary>
    /// List of messages read from json
    /// </summary>
    [System.NonSerialized]
    public (string, float)[] messages;

    /// <summary>
    /// Used to track which message to display next
    /// </summary>
    private int _messageIndex = 0;

    /// <summary>
    /// Dictionary converting directions to vectors of logical positions
    /// </summary>
    /// <value>4 cardinal directions</value>
    [System.NonSerialized]
    public Dictionary<Direction, Vector2Int> logicalDirections = new Dictionary<Direction, Vector2Int>
    {
        {Direction.North, new Vector2Int(0,-1)},
        {Direction.South, new Vector2Int(0,1)},
        {Direction.East, new Vector2Int(1,0)},
        {Direction.West, new Vector2Int(-1,0)}
    };

    /// <summary>
    /// Reference tracking the flip coroutine
    /// </summary>
    private Coroutine _flipCoroutine;

    /// <summary>
    /// Distance to travel when a tile needs to flip
    /// </summary>
    public float flipDistance;

    /// <summary>
    /// Duration of travel when a flip is performed
    /// </summary>
    public float flipDuration;

        /// <summary>
    /// Event of the player touching an orb
    /// </summary>
    public UnityEvent TouchOrb;

    /// <summary>
    /// Flag to know if the board is flipping
    /// </summary>
    private bool _flipping = false;
    
    /// <summary>
    /// Flag to know if the last move was a flip
    /// </summary>
    private bool _justflipped;

    /// <summary>
    /// Reference to the orb count UI
    /// </summary>
    private GameObject orbCountUI;
    
    /// <summary>
    /// Reference to the orb message UI
    /// </summary>
    private GameObject orbMessageUI;

    private GameObject pauseMenuUI;

    /// <summary>
    /// Sets game object references
    /// </summary>
    private void Awake()
    {
        var gm = GameManager.GetComponent<GameManager>();
        bearPrefab = gm.BearPrefab;
        orbPrefab = gm.OrbPrefab;
        tilePrefab = gm.TilePrefab;
        orbCountUI = gm.OrbCountUI;
        orbMessageUI = gm.OrbMessageUI;
        pauseMenuUI = gm.PauseMenuUI;
    }

    /// <summary>
    /// Destroys the game object loaded by Level(str)
    /// </summary>
    public void TearDown()
    {
        foreach (var tile in tiles)
        {
            DestroyImmediate(tile);
        }

        if (orbs != null)
        foreach (var orb in orbs)
        {
            if (orb != null) DestroyImmediate(orb);
        }

        if (characters != null)
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
        SetUpMessages(json);

        SetUpTiles(json);

        SetUpOrbs(json);

        SetUpCharacters(json);
    }

    /// <summary>
    /// Sets up the messages of the level
    /// </summary>
    /// <param name="json"></param>
    private void SetUpMessages(JSONLevel json)
    {
        this.messages = new (string, float)[json.Messages.Length];

        for (int i = 0; i < this.messages.Length; i++)
        {
            this.messages[i] = (json.Messages[i].Message, json.Messages[i].Duration);
        }
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

            float newTileZCoord;

            // set its state
            switch (json.Tiles[i])
            {
                case 1:
                    newTileZCoord = 1;
                    break;
                case 2:
                    newTileZCoord = 0;
                    break;
                default:
                    throw new System.ArgumentException($"Invalid tile state, seen {json.Tiles[i]}");
            }

            var newTile =
                GameObject.Instantiate(tilePrefab, new Vector3(x, boardDimension[1] - y - 1, newTileZCoord),
                    Quaternion.Euler(0, 0, 0), this.transform);

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
            this.tileCount++;
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
            // Get parent tile
            var orb = json.Orbs[i];
            var tile = tiles[orb.Coord[0], orb.Coord[1]];
            if (tile == null) throw new System.Exception($"There is no tile on {orb.Coord[0]}, {orb.Coord[1]}");
            var parent = tile.transform;

            orbs[i] = GameObject.Instantiate(orbPrefab,
                new Vector3(0, 0, -1),
                Quaternion.Euler(0, 0, 0), parent);
            orbs[i].transform.localPosition = new Vector3(0, 0, -1);
        }

        orbCountUI.GetComponent<OrbCountUI>().InitiateCount(orbs.Length);
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
        if (pauseMenuUI.GetComponentInParent<PauseMenu>().GameIsPaused) return;

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

        foreach (var orb in orbs)
        {
            if (orb == null || orb.activeSelf == false) continue;
            if (orb.transform.position.x == location.x && orb.transform.position.y == location.y)
            {
                // contact made between player and orb
                // contact is only possible when the orb is active

                // there is a contact orb
                orb.GetComponent<Orb>().Touched();
                return true;
            }
        }

        // no contact case
        return false;
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    public void DisplayOrbMessage()
    {
        Debug.Log(this.messages[_messageIndex]);
        

        orbCountUI.GetComponent<OrbCountUI>().Increment();

        (var message, var duration) = this.messages[_messageIndex];
        orbMessageUI.GetComponent<OrbMessageUI>().Display(message, duration);
        _messageIndex++;
    }

    /// <summary>
    /// Checks if the given move is legal according to the game state
    /// </summary>
    /// <param name="logicalCoordinate">Logical position</param>
    /// <param name="to">Direction to move to</param>
    /// <returns>Whether the move is legal</returns>
    public bool IsLegalMove(Vector2Int logicalCoordinate, Direction to)
    {
        // checks if the bear is located within proper coordinates
        VerifyLogicalCoordinate(logicalCoordinate, true);

        var destination = logicalCoordinate + logicalDirections[to];

        // destination is valid in the game
        if (VerifyLogicalCoordinate(destination, false))
        {
            var fromTile = tiles[logicalCoordinate[0], logicalCoordinate[1]]
                .GetComponent<Tile>();
            var toTile = tiles[destination[0], destination[1]]
                .GetComponent<Tile>();

            // if the tile is moving (lowering), then the move is illegal
            if (toTile.Lowering) return false;

            // if the tile is fading, then the move is illegal
            if (toTile.Fading) return false;

            // check if the tile is at the same level as the logicalCoordinate
            if (fromTile.State == toTile.State) return true;
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
    /// Converts the given coordinate into logical coordinate of the board
    /// </summary>
    /// <param name="worldCoordinate">World coordinate to convert from</param>
    /// <returns>Converted logical coordinate</returns>
    public Vector2Int ToLogicalCoordinates(Vector3 worldCoordinate)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldCoordinate.x),
            Mathf.RoundToInt(this.boardDimension[1] - 1 - worldCoordinate.y)
        );
    }

    /// <summary>
    /// Lowers the tile that the player just moved out of. If marked for destruction, initiate destruction.
    /// </summary>
    /// <param name="performedMoveDirection">Direction taken by the player</param>
    public void ActOnPreviousTile(Direction performedMoveDirection)
    {
        var previousLogicalPosition = player.GetComponent<Bear>().LogicalPosition
            - logicalDirections[performedMoveDirection];
        
        // check if a tile needs to be destroyed because of a flip
        if (_justflipped)
        {
            tiles[previousLogicalPosition[0], previousLogicalPosition[1]].GetComponent<Tile>()
                .DestroyAfterFlipping();
            
            _justflipped = false;
        }
        else
            tiles[previousLogicalPosition[0], previousLogicalPosition[1]].GetComponent<Tile>()
                .LowerTile();
    }

    /// <summary>
    /// Flips the board, a signature move
    /// </summary>
    public void Flip()
    {
        if (_flipping == true) return;

        float speed = flipDistance / flipDuration;

        _flipCoroutine = GameManager.GetComponent<GameManager>().navigator.Flip(
            flipDuration, speed, this,
            () => _flipping = true,
            () => {
                _flipping = false; 
                _justflipped = true;
            },
            () => false
        );
    }

    /// <summary>
    /// Listener of the event of flipping the board
    /// </summary>
    /// <param name="context"></param>
    public void Flip(InputAction.CallbackContext context)
    {
        if (pauseMenuUI.GetComponentInParent<PauseMenu>().GameIsPaused) return;

        if (context.performed)
        {
            Flip();
        }
    }
}
