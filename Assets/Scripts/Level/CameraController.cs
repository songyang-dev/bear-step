using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Script that controls the camera, including setting limits to its position
/// </summary>
public class CameraController : MonoBehaviour
{

    /// <summary>
    /// Limits object positions, each limit is the camera's centered position on an 'edged' object
    /// </summary>
    /// <typeparam name="Direction">The limit's direction</typeparam>
    /// <typeparam name="float">World space location</typeparam>
    /// <returns></returns>
    private Dictionary<Direction, float> _limits = new Dictionary<Direction, float>(4);

    /// <summary>
    /// Positions of the furthest game coordinates in the scene
    /// </summary>
    /// <typeparam name="Direction">Direction furthest away to</typeparam>
    /// <typeparam name="Vector3">World coordinate</typeparam>
    /// <returns></returns>
    private Dictionary<Direction, Vector3> _edgeCoordinates = new Dictionary<Direction, Vector3>(4);

    /// <summary>
    /// Original position of the camera compared to the player
    /// </summary>
    public Vector3 Offset;

    /// <summary>
    /// Original orientation of the camera compared to the player
    /// </summary>
    public Vector3 Rotation;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    public GameObject gameManager;

    /// <summary>
    /// Board of the scene
    /// </summary>
    private Board _board;

    /// <summary>
    /// Movement direction that is temporarily stored
    /// </summary>
    private Vector2 _moving;

    /// <summary>
    /// Speed of moving the camera, units/second
    /// </summary>
    public float Speed;

    /// <summary>
    /// Camera component
    /// </summary>
    private Camera _camera;

    private void Awake()
    {
        _board = gameManager.GetComponent<GameManager>().Board.GetComponent<Board>();
        _camera = this.GetComponent<Camera>();
    }

    /// <summary>
    /// Sets up where the camera starts and its limits
    /// </summary>
    /// <param name="json">Serialized json class of the level</param>
    public void SetUp()
    {
        
        // reinitialize the limits
        _limits.Clear();
        
        FindEdgeCoordinates();
        CalculateLimits();

        // center camera
        Center();
    }

    /// <summary>
    /// Computes the limits of the camera's position in world space
    /// </summary>
    private void CalculateLimits()
    {
        // Determine the limits of the camera in viewport screen space

        // southern limit is the most negative y coordinate for the camera
        this._limits.Add(Direction.South,
            (this._edgeCoordinates[Direction.South] + this.Offset).y
        );

        // northern limit is the most positive y coordinate for the camera
        this._limits.Add(Direction.North, 
            (this._edgeCoordinates[Direction.North] + this.Offset).y
        );

        // western limit is the most negative x coordinate for the camera
        this._limits.Add(Direction.West, 
            (this._edgeCoordinates[Direction.West] + this.Offset).x
        );

        // eastern limit is the most positive x coordinate for the camera
        this._limits.Add(Direction.East,
            (this._edgeCoordinates[Direction.East] + this.Offset).x
        );
    }

    /// <summary>
    /// Listener to the event of moving the camera, applies limits when stops moving
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        _moving = context.action.ReadValue<Vector2>();
        
        if (_moving.Equals(Vector2.zero)) ApplyLimits();
    }

    /// <summary>
    /// Moves the camera according to the field _moving
    /// </summary>
    public void Move()
    {
        if (_moving.Equals(Vector2.zero)) return;
        
        var change = new Vector3(_moving.x * Speed * Time.deltaTime,
            _moving.y * Speed * Time.deltaTime, 0);

        this.transform.Translate(change, Space.World);
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    /// <param name="step"></param>
    public void Move(Vector2 step)
    {
        var change = new Vector3(step.x * Speed * Time.deltaTime,
            step.y * Speed * Time.deltaTime, 0);

        this.transform.Translate(change, Space.World);
        ApplyLimits();
    }

    /// <summary>
    /// Restricts the position of the camera within the board. 
    /// </summary>
    private void ApplyLimits()
    {
        var clamped = this.transform.position;
        clamped.x = Mathf.Clamp(clamped.x, this._limits[Direction.West], this._limits[Direction.East]);
        clamped.y = Mathf.Clamp(clamped.y, this._limits[Direction.South], this._limits[Direction.North]);
        this.transform.position = clamped;
    }

    /// <summary>
    /// Calculates where the furthest coordinates are located
    /// </summary>
    private void FindEdgeCoordinates()
    {
        var boardDimension = _board.GetComponent<Board>().boardDimension;

        // always 0 in y coordinate
        this._edgeCoordinates.Add(Direction.South, new Vector3(0, 0, 0));
        // in the +y direction in world space
        this._edgeCoordinates.Add(Direction.North, new Vector3(0, boardDimension[1] - 1, 0));

        // always 0 in x coordinate
        this._edgeCoordinates.Add(Direction.West, new Vector3(0, 0, 0));
        // in the +x direction in world space
        this._edgeCoordinates.Add(Direction.East,new Vector3(boardDimension[0] - 1, 0, 0));
    }

    /// <summary>
    /// Listener to the event of centering the camera
    /// </summary>
    /// <param name="context"></param>
    public void Center(InputAction.CallbackContext context)
    {
        if (context.performed) Center();
    }

    /// <summary>
    /// Centers the camera on the player, restoring default rotation.
    /// Then applies camera limits.
    /// </summary>
    public void Center()
    {
        var playerPosition = _board.GetComponent<Board>().player.transform.position;

        this.transform.SetPositionAndRotation(playerPosition + Offset,
            Quaternion.Euler(Rotation));

        ApplyLimits();
    }

    void Update()
    {
        // move the camera
        Move();
    }
}
