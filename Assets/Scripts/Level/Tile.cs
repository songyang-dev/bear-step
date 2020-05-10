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
    private TileState state;

    public TileState State { get => state; private set => state = value; }

    public float moveDistance;

    public float moveDuration;

    private Coroutine _movingCoroutine;

    public GameManager gameManager;

    private void Awake()
    {
        gameManager = this.GetComponentInParent<Board>().gameManager.GetComponent<GameManager>();
    }

    public void InitiateTileState(TileState state)
    {
        State = state;
    }

    public void ChangeTileState(TileState state)
    {
        if (state == this.State) throw new System.Exception($"Tile state did not change from {state}");
        State = state;

        Vector3 change = new Vector3();
        // physically lower the tile
        switch (state)
        {
            case TileState.Down:
                change.z = 1;
                break;

            case TileState.Up:
                change.z = -1;
                break;

            default:
                throw new System.Exception($"Unknown tile state to be changed to: {state}");
        }

        _movingCoroutine = gameManager.Move(moveDuration, moveDistance, this.transform.position + change,
            this.transform, () =>
            {
                State = state;
            }
        );
    }
}
