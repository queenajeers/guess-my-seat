using UnityEngine;

public static class GameData
{
    public static int CoinsToUseHint = 200;
    public static int CurrentLevel
    {
        get => PlayerPrefs.GetInt("CurrentLevel", 1); // Default: 1
        set => PlayerPrefs.SetInt("CurrentLevel", value);
    }

    public static int Hints
    {
        get => PlayerPrefs.GetInt("Hints", 4); // Default: 0
        set => PlayerPrefs.SetInt("Hints", value);
    }

    public static int Coins
    {
        get => PlayerPrefs.GetInt("Coins", 0); // Default: 0
        set => PlayerPrefs.SetInt("Coins", value);
    }

    public static int Lives
    {
        get => PlayerPrefs.GetInt("Lives", 2); // Default: 5
        set => PlayerPrefs.SetInt("Lives", value);
    }

}
