using UnityEngine;
using System.IO;

public static class GameData
{
    public static int CoinsToUseHint = 200;

    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt("CurrentLevel", 0); // Default: 1
        set => PlayerPrefs.SetInt("CurrentLevel", value);
    }

    public static int Hints
    {
        get => PlayerPrefs.GetInt("Hints", 4); // Default: 4
        set => PlayerPrefs.SetInt("Hints", value);
    }

    public static int Coins
    {
        get => PlayerPrefs.GetInt("Coins", 0); // Default: 0
        set => PlayerPrefs.SetInt("Coins", value);
    }

    public static int Lives
    {
        get => PlayerPrefs.GetInt("Lives", 2); // Default: 2
        set => PlayerPrefs.SetInt("Lives", value);
    }

    public static bool IsNewLevel(int levelNumber)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels", levelNumber.ToString(), $"Level_{levelNumber}.json");
        return !File.Exists(path); // If it doesn't exist, it's a new level (new to the player)
    }
}
