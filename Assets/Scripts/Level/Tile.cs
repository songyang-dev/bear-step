using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State of a tile, UP is always the state of the tile under the player
/// </summary>
public enum TileState
{
    Up,
    Down
}

public class Tile : MonoBehaviour
{
    /// <summary>
    /// Underlying tile state
    /// </summary>
    private TileState _state;

    /// <summary>
    /// Exposed property of the tile state
    /// </summary>
    /// <value></value>
    public TileState State { get => _state; private set => _state = value; }

    /// <summary>
    /// Whether the tile is lowering
    /// </summary>
    /// <value></value>
    public bool Lowering { get => _lowering; private set => _lowering = value; }
    
    public bool Fading { get => _fading; private set => _fading = value; }

    /// <summary>
    /// Distance to move when changing state
    /// </summary>
    public float StateChangeDistance;

    /// <summary>
    /// Duration to move when changing state
    /// </summary>
    public float StateChangeDuration;

    /// <summary>
    /// Reference to track the coroutine of moving the tile
    /// </summary>
    private Coroutine _movingCoroutine;

    /// <summary>
    /// Reference to the game manager script
    /// </summary>
    private GameManager gameManager;

    /// <summary>
    /// Internal flag for telling if the tile is lowering
    /// </summary>
    private bool _lowering;

    /// <summary>
    /// World coordinate positions to go when trying to reach a certain tile state
    /// </summary>
    /// <value></value>
    public Dictionary<TileState, Vector3> tileStatePositions;
    private bool _fading;
    private Coroutine _fade;

    private void Awake()
    {
        gameManager = this.GetComponentInParent<Board>().GameManager.GetComponent<GameManager>();

    }

    /// <summary>
    /// Used when creating the board
    /// </summary>
    /// <param name="state">Initial state of the board</param>
    public void InitiateTileState(TileState state)
    {
        State = state;

        switch (State)
        {
            case TileState.Down:
                tileStatePositions = new Dictionary<TileState, Vector3> {
                    {TileState.Down, this.transform.position},
                    {TileState.Up, this.transform.position + new Vector3(0,0,-1)}
                };
                break;

            case TileState.Up:
                tileStatePositions = new Dictionary<TileState, Vector3> {
                    {TileState.Down, this.transform.position + new Vector3(0,0,1)},
                    {TileState.Up, this.transform.position}
                };
                break;

            default:
                throw new System.Exception($"Unknown tile state in Tile.Awake: {State}");
        }
    }

    /// <summary>
    /// Lowers the tile, changing its state
    /// </summary>
    public void LowerTile()
    {
        if (State == TileState.Down) throw new System.Exception($"Tile state was already down");

        var speed = StateChangeDistance / StateChangeDuration;

        // physically lower the tile
        _movingCoroutine = gameManager.navigator.LowerTile(StateChangeDuration, speed, tileStatePositions[TileState.Down],
            this.transform,
            () => { Lowering = true; }, // before
            () => // after
            {
                State = TileState.Down;
                Lowering = false;
            },
            () => false // do not force when access is denied
        );
    }

    /// <summary>
    /// Changes the state of the tile to its flipped state
    /// </summary>
    public void Flip()
    {
        switch (State)
        {
            case TileState.Down:
                State = TileState.Up;
                break;

            case TileState.Up:
                State = TileState.Down;
                break;

            default:
                throw new System.Exception($"Unrecognized tile state in flip: {State}");
        }
    }

    /// <summary>
    /// Calculates where the flipped position would be
    /// </summary>
    /// <returns>Flipped position</returns>
    public Vector3 FlipPosition()
    {
        switch (State)
        {
            case TileState.Down:
                return tileStatePositions[TileState.Up];

            case TileState.Up:
                return tileStatePositions[TileState.Down];

            default:
                throw new System.Exception("Unrecognized tile state in FlipPosition()");
        }
    }


    public void DestroyAfterFlipping()
    {
        Fading = true;
        _fade = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var renderer = GetComponent<Renderer>();

        for (float ft = 1f; ft >= 0; ft -= 0.1f)
        {
            Color c = renderer.material.color;
            c.a = ft;
            renderer.material.color = c;
            yield return null;
        }

        Fading = false;
        GetComponentInParent<Board>().tileCount--; // decrease tile count
        Destroy(this.gameObject);
    }
}
