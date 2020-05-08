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

    public TileState state;

}
