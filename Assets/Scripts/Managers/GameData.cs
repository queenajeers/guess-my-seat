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

    public static int LevelsPlayed
    {
        get => PlayerPrefs.GetInt("LevelsPlayed", 0); // Default: 0
        set => PlayerPrefs.SetInt("LevelsPlayed", value);
    }
    public static int SolvedOnFirstTry
    {
        get => PlayerPrefs.GetInt("SolvedOnFirstTry", 0); // Default: 0
        set => PlayerPrefs.SetInt("SolvedOnFirstTry", value);
    }
    public static int MinutesPlayed
    {
        get => PlayerPrefs.GetInt("MinutesPlayed", 0); // Default: 0
        set => PlayerPrefs.SetInt("MinutesPlayed", value);
    }

    public static int IQ
    {
        get => PlayerPrefs.GetInt("IQ", 70); // Default: 0
        set => PlayerPrefs.SetInt("IQ", value);
    }

    public static bool IsNewLevel(int levelNumber)
    {
        string path = Path.Combine(Application.persistentDataPath, "Levels", levelNumber.ToString(), $"Level_{levelNumber}.json");
        return !File.Exists(path); // If it doesn't exist, it's a new level (new to the player)
    }


}
