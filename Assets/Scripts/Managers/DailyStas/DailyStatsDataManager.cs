using System;
using UnityEngine;

public class DailyStatsDataManager : MonoBehaviour
{
    public static DailyStatsDataManager Instance { get; private set; }


    DateTime beginTime;
    DateTime endTime;

    bool finishedOnFirstTry = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LevelStarted()
    {
        beginTime = DateTime.Now;
        finishedOnFirstTry = true;
    }

    public void WrongChoice()
    {
        finishedOnFirstTry = false;
    }


    public void LevelFinished()
    {
        endTime = DateTime.Now;
        UpdateTotalDurationInSeconds();

        if (finishedOnFirstTry)
        {
            GameData.SolvedOnFirstTry++;
        }

        GameData.LevelsPlayed++;
    }

    public void UpdateTotalDurationInSeconds()
    {

        TimeSpan duration = endTime - beginTime;
        float totalDurationInSeconds = (float)duration.TotalSeconds;
        GameData.TimeSpentInSeconds += totalDurationInSeconds;

        // Convert to minutes
        int minutesPlayed = (int)(GameData.TimeSpentInSeconds / 60);
        GameData.MinutesPlayed = minutesPlayed;

    }
}
