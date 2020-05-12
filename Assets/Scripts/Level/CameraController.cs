using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{

    /// <summary>
    /// Limits of the camera's positions
    /// </summary>
    /// <typeparam name="Direction">The limit's direction</typeparam>
    /// <typeparam name="float">Coordinate component value of the direction's limit</typeparam>
    /// <returns></returns>
    private Dictionary<Direction, float> limits = new Dictionary<Direction, float>(4);

    /// <summary>
    /// Original position of the camera compared to the player
    /// </summary>
    public Vector3 offset;

    /// <summary>
    /// Original orientation of the camera compared to the player
    /// </summary>
    public Vector3 rotation;

    /// <summary>
    /// Board gameobject of the scene
    /// </summary>
    public GameObject board;

    /// <summary>
    /// Movement direction that is temporarily stored
    /// </summary>
    private Vector2 _moving;

    /// <summary>
    /// Speed of moving the camera, units/second
    /// </summary>
    public float speed;

    /// <summary>
    /// Sets up where the camera starts and its limits
    /// </summary>
    /// <param name="json">Serialized json class of the level</param>
    public void SetUp(JSONLevel json)
    {
        var _board = board.GetComponent<Board>();

        limits.Clear();

        // north and west boundaries are always the same
        limits.Add(Direction.North, offset.y + 2);
        limits.Add(Direction.West, offset.x - 2);

        // south boundary is in the -y direction
        limits.Add(Direction.South,
            offset.y - _board.boardDimension[1]/2);

        // east boundary is in the +x direction
        limits.Add(Direction.East,
            offset.x + _board.boardDimension[0]);

        // center camera
        Center();
    }

    /// <summary>
    /// Listener to the event of moving the camera
    /// </summary>
    /// <param name="context"></param>
    public void Move(InputAction.CallbackContext context)
    {
        _moving = context.action.ReadValue<Vector2>();
        //Debug.Log(_moving);
    }

    /// <summary>
    /// Moves the camera according to the field _moving, applies limits after
    /// </summary>
    public void Move()
    {
        if (_moving.Equals(Vector2.zero)) return;
        
        var change = new Vector3(_moving.x * speed * Time.deltaTime,
            _moving.y * speed * Time.deltaTime, 0);

        this.transform.Translate(change, Space.World);
        ApplyLimits();
    }

    /// <summary>
    /// For testing purposes
    /// </summary>
    /// <param name="step"></param>
    public void Move(Vector2 step)
    {
        var change = new Vector3(step.x * speed * Time.deltaTime,
            step.y * speed * Time.deltaTime, 0);

        this.transform.Translate(change, Space.World);
        ApplyLimits();
    }

    /// <summary>
    /// Restrict the position of the camera within the board
    /// </summary>
    private void ApplyLimits()
    {
        var current = this.transform.position;
        current.x = Mathf.Clamp(current.x, limits[Direction.West], limits[Direction.East]);
        current.y = Mathf.Clamp(current.y, limits[Direction.South], limits[Direction.North]);
        this.transform.position = current;
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
        var playerPosition = board.GetComponent<Board>().player.transform.position;

        this.transform.SetPositionAndRotation(playerPosition + offset,
            Quaternion.Euler(rotation));

        ApplyLimits();
    }

    void Update()
    {
        // move the camera
        Move();
    }
}
