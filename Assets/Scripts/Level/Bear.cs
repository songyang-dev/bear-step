using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class Bear : MonoBehaviour
{
    /// <summary>
    /// Temporary variable storing the movement direction for the coroutine Move 
    /// </summary>
    private Direction? _going;

    /// <summary>
    /// Temporary variable storing the movement destination, used to stabilize floating points
    /// </summary>
    private Vector3 _destination;

    /// <summary>
    /// Dictionary holding the equivalencies of direction with the WORLD coordinates
    /// </summary>
    /// <typeparam name="Direction">Cardinal direction in the game</typeparam>
    /// <typeparam name="Vector3">Unit vector in WORLD frame</typeparam>
    /// <returns></returns>
    public Dictionary<Direction, Vector3> directions = new Dictionary<Direction, Vector3>() {
        {Direction.East, Vector3.right},
        {Direction.West, Vector3.left},
        {Direction.North, Vector3.up},
        {Direction.South, Vector3.down}
    };

    /// <summary>
    /// Duration taken by the player to move from one tile to the next
    /// </summary>
    public float moveDuration;

    /// <summary>
    /// Distance per player move, usually one
    /// </summary>
    public float moveDistance;

    /// <summary>
    /// Reference to the coroutine of move
    /// </summary>
    private Coroutine _movingCoroutine;

    /// <summary>
    /// Reference to the board
    /// </summary>
    public Board board;

    /// <summary>
    /// Position of the bear in game logic (read only)
    /// </summary>
    /// <returns></returns>
    public Vector2Int logicalPosition = new Vector2Int();

    public GameManager gameManager;

    /// <summary>
    /// Sets the references to the board and the player's logical position
    /// </summary>
    private void Awake()
    {
        board = this.GetComponentInParent<Board>();
        gameManager = board.gameManager.GetComponent<GameManager>();
        UpdateBoardPosition();
    }

    /// <summary>
    /// Updates the player's logical position based on physical position
    /// </summary>
    private void UpdateBoardPosition()
    {
        logicalPosition = board.ToLogicalCoordinates(this.transform.position);
    }

    /// <summary>
    /// Initiates a coroutine based on the given direction
    /// </summary>
    /// <param name="direction">Direction to move to</param>
    public void Move(Direction direction)
    {
        if (_going != null) return;
        if (!board.IsLegalMove(this.logicalPosition, direction)) return;

        _going = direction;
        _destination = this.transform.position + directions[direction];

        // must check for opened directions
        _movingCoroutine = gameManager.Move(moveDuration, moveDistance, _destination, this.transform,
            () =>
            {
                UpdateBoardPosition();

                // signal the board (parent) for orb collisions
                // signal the board (parent) for tile state changes
                board.PlayerMoved();
                board.LowerPreviousTile(direction);

                _going = null;
            }
        );
    }

    /// <summary>
    /// Event listener to the user input of moving the player
    /// </summary>
    /// <param name="context">Unity input system callback context</param>
    public void Move(InputAction.CallbackContext context)
    {
        if (_going != null) return;

        var read = context.action.ReadValue<Vector2>();

        if (read.x == 0 && read.y == 0) return;

        if (Mathf.Abs(read.x) >= Mathf.Abs(read.y))
        {
            if (read.x > 0) Move(Direction.East);
            else Move(Direction.West);
        }
        else
        {
            if (read.y > 0) Move(Direction.North);
            else Move(Direction.South);
        }
    }
}
