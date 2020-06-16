using UnityEngine;

/// <summary>
/// Class to keep track of the player's progress in the game
/// </summary>
public class PlayerProgress
{
    /// <summary>
    /// Name of the player/save file
    /// </summary>
    private string _name;

    /// <summary>
    /// JSON of the campaign
    /// </summary>
    private JSONCampaign _campaign;

    /// <summary>
    /// Number of levels done
    /// </summary>
    private int _levelIndex;

    /// <summary>
    /// Initiates a new player progress
    /// </summary>
    /// <param name="name">Name of the player/save file</param>
    public PlayerProgress(string name)
    {
        _name = name;
        var json = Resources.Load<TextAsset>("Campaign/Campaign");
        _campaign = JsonUtility.FromJson<JSONCampaign>(json.text);
        _levelIndex = 0;
    }

    /// <summary>
    /// Returns the name of the next level, null if no level
    /// </summary>
    /// <returns>Next level's name or null</returns>
    public string GetNextLevelName()
    {
        try {
            return _campaign.Levels[_levelIndex + 1];
        }
        catch (System.IndexOutOfRangeException)
        {
            return null;
        }
    }

    /// <summary>
    /// Returns the name of the current level
    /// </summary>
    /// <returns>Current level name</returns>
    public string GetCurrentLevelName()
    {
        return _campaign.Levels[_levelIndex];
    }

    /// <summary>
    /// Changes the PlayerPrefs to reflect progressing to the next level
    /// </summary>
    public void MoveToNextLevel()
    {
        _levelIndex++;
        PlayerPrefs.SetInt($"{_name}_levelIndex", _levelIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Deletes the player progress' footprint in PlayerPrefs
    /// </summary>
    public void RemoveProgress()
    {
        PlayerPrefs.DeleteKey($"{_name}_levelIndex");
    }
}