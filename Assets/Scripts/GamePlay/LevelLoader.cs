using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{

    [Serializable]
    public class SeatData
    {
        public int row;
        public int col;
        public string personSeating;
        public string hint;
        public string seatNumber;
        public bool isInitiallyOpened;
    }

    [Serializable]
    public class LevelData
    {
        public int rows;
        public int cols;
        public List<SeatData> seats;
        public SeatData[,] seatsGrid;

        public void ConvertSeatsToSeatsGrid()
        {
            seatsGrid = new SeatData[rows, cols];

            foreach (SeatData seat in seats)
            {
                if (seat.row >= 0 && seat.row < rows && seat.col >= 0 && seat.col < cols)
                {
                    seatsGrid[seat.row, seat.col] = seat;
                }
                else
                {
                    Debug.LogWarning($"Seat {seat.seatNumber} has invalid position: row {seat.row}, col {seat.col}");
                }
            }
        }
    }

    [Header("JSON Level File")]
    public TextAsset jsonFile;

    [Header("Level Building Blocks")]
    [SerializeField] private GameObject seatPrefab;

    private LevelData levelData;

    void Start()
    {
        if (jsonFile == null || seatPrefab == null)
        {
            Debug.LogError("Missing JSON file or Seat Prefab in inspector.");
            return;
        }

        LoadLevelData(jsonFile.text);
        InitialiseLevel();
    }

    public void LoadLevelData(string json)
    {
        levelData = JsonUtility.FromJson<LevelData>(json);
        Debug.Log($"Rows: {levelData.rows}, Cols: {levelData.cols}");
        Debug.Log($"Seats Count: {levelData.seats.Count}");
        levelData.ConvertSeatsToSeatsGrid();
    }

    void InitialiseLevel()
    {
        var seatData = levelData.seatsGrid;
        for (int x = 0; x < seatData.GetLength(0); x++)
        {
            for (int y = 0; y < seatData.GetLength(1); y++)
            {
                var currentSeat = seatData[x, y];
                if (currentSeat == null)
                {
                    Debug.LogWarning($"Seat at [{x}][{y}] is null, skipping.");
                    continue;
                }

                Vector2 spawnPos = new Vector2(2f * y, -2f * x); // y = col, x = row
                GameObject seatObj = Instantiate(seatPrefab, spawnPos, Quaternion.identity);
                Seat seatComponent = seatObj.GetComponent<Seat>();

                if (seatComponent != null)
                {
                    seatComponent.LoadData(currentSeat.personSeating, currentSeat.seatNumber, currentSeat.hint);
                    SeatSelector.Instance.AddSeat(seatComponent);
                }
                else
                {
                    Debug.LogError("Seat prefab missing 'Seat' component.");
                }
            }
        }
    }
}
