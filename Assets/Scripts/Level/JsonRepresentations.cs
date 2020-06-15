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
    public JSONMessage[] Messages;

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
    /// <summary>
    /// Location on the board. [0,0] is the top left corner
    /// </summary>
    public int[] Coord;

    public void Verify(int[] dimension)
    {
        if (Coord == null) return;

        if (Coord.Length != 2)
            throw new System.ArgumentException("Orb coordinate array does not have length 2, but " + Coord.Length);

        if (Coord[0] < 0 || Coord[1] < 0)
            throw new System.ArgumentException($"Orb coords are negative: {Coord[0]}, {Coord[1]}");

        if (Coord[0] >= dimension[0] || Coord[1] >= dimension[1])
            throw new System.ArgumentException($"Orb coords are out of bounds: {Coord[0]}, {Coord[1]}");
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

[System.Serializable]
public class JSONMessage
{
    public string Message;

    public float Duration;

    public void Verify()
    {
        if (Duration <= 0)
            throw new System.ArgumentException($"Duration of the message is negative: {Duration}");
    }
}

