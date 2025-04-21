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
    public int levelToLoad;

    [Header("Level Building Blocks")]
    [SerializeField] private GameObject seatPrefab;

    private LevelData levelData;

    void Start()
    {
        var jsonFile = Resources.Load<TextAsset>($"Levels/{levelToLoad}/Level_{levelToLoad}");

        if (jsonFile == null || seatPrefab == null)
        {
            Debug.LogError("Missing JSON file or Seat Prefab in inspector.");
            return;
        }

        GamePlayManager.Instance.SetLevel(levelToLoad);
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
        var allPersons = new List<string>();
        var openPersons = new List<string>();
        List<Seat> seatBounds = new List<Seat>();

        foreach (var seat in seatData)
        {
            allPersons.Add(seat.personSeating.Trim());
        }

        Vector2 seatPositionsMean = Vector2.zero;
        var allSeats = new Transform[seatData.GetLength(0), seatData.GetLength(1)];

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

                GameObject seatObj = Instantiate(seatPrefab, Vector2.zero, Quaternion.identity);
                allSeats[x, y] = seatObj.transform;

                if (seatObj.TryGetComponent<Seat>(out var seatComponent))
                {
                    var seatBound = seatComponent.GetBounds();
                    seatBounds.Add(seatComponent);

                    Vector2 spawnPos = new((seatBound.size.x + 0.1f) * y, -(seatBound.size.y + 0.1f) * x); // y = col, x = row
                    seatObj.transform.position = spawnPos;
                    seatPositionsMean += spawnPos;
                    PersonData personData = Resources.Load<PersonData>($"Levels/{levelToLoad}/{currentSeat.personSeating.Trim()}");

                    string hyperHintText = TextStyler.GiveHyperText(currentSeat.hint, allPersons);
                    seatComponent.LoadData(currentSeat.personSeating, personData.personIcon, currentSeat.seatNumber, hyperHintText);

                    if (!currentSeat.isInitiallyOpened)
                    {
                        SeatSelector.Instance.AddSeat(seatComponent);
                    }
                    else
                    {
                        openPersons.Add(currentSeat.personSeating);
                        seatComponent.isOpenSeat = true;

                    }
                }
                else
                {
                    Debug.LogError("Seat prefab missing 'Seat' component.");
                }
            }
        }

        // Spawning People

        var seatsCenterPos = seatPositionsMean / allPersons.Count;

        GameObject seatsCentre = new GameObject("AllSeats");

        seatsCenterPos.y = allSeats[0, 0].transform.position.y + (seatBounds[0].GetBounds().size.y / 2f);

        if (seatData.GetLength(1) > 3)
        {
            seatsCenterPos.x = allSeats[0, 0].transform.position.x - (seatBounds[0].GetBounds().size.x / 2f);
        }

        seatsCentre.transform.position = seatsCenterPos;


        foreach (var item in allSeats)
        {
            item.SetParent(seatsCentre.transform);
        }
        if (seatData.GetLength(1) <= 3)
        {
            seatsCentre.transform.position = VptoWP(0.5f, 1f) - new Vector2(0, 2.5f);
        }
        else
        {
            seatsCentre.transform.position = VptoWP(0f, 1f) + new Vector2(0.25f, -2f);
        }

        for (int p = 0; p < levelData.seats.Count; p++)
        {
            PersonData personData = Resources.Load<PersonData>($"Levels/{levelToLoad}/{levelData.seats[p].personSeating.Trim()}");
            if (personData != null && !openPersons.Contains(personData.personName))
            {
                UIManager.Instance.SpawnNewPerson(personData.personIcon, personData.personName);
            }
        }

        CameraDragMove.Instance.AdjustBoundary(seatBounds);
        SeatSelector.Instance.InitialseSeats(openPersons.Count, allPersons.Count);

    }

    Vector2 VptoWP(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new(x, y, 0));

    }
}
