using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance { get; private set; }

    [Serializable]
    public class SeatData
    {
        public int row;
        public int col;
        public string seatNumber;
        public string personName;
        public string personGender;
        public string hint;
        public bool isInitiallyOpened;
        public List<string> linkedSeats;
        public List<string> tags;
    }

    [Serializable]
    public class LevelData
    {
        public string levelName;
        public int rows;
        public int cols;
        public List<SeatData> seats;
        [NonSerialized] public SeatData[,] seatsGrid;

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


    [Header("Level Building Blocks")]
    [SerializeField] private GameObject seatPrefab;

    private LevelData levelData;

    List<Seat> allSeatComponents = new List<Seat>();
    List<string> placedSeats = new List<string>();

    Dictionary<string, PersonItem> personItemsByIds = new Dictionary<string, PersonItem>();
    Dictionary<string, string> personSeatIdsByName = new Dictionary<string, string>();

    Dictionary<string, Seat> seatCompsByName = new Dictionary<string, Seat>();


    [SerializeField] int levelToLoad;

    void Awake()
    {
        Instance = this;
        levelToLoad = GameData.CurrentLevel;

    }
    void Start()
    {
        if (PlayerPrefs.GetInt("FirstTime", 0) == 0)
        {
            PlayerPrefs.SetInt("FirstTime", 1);
            PlayerPrefs.Save();
            DeletePersistentLevelsPath();
        }

        if (GameData.CurrentLevel == 0)
        {
            levelToLoad = 0;
            DeletePersistentLevelsPath();
        }

        // Get all level JSON files in Resources/Levels and find max level number
        TextAsset[] allLevelFiles = Resources.LoadAll<TextAsset>("Levels");
        int maxLevel = allLevelFiles
            .Select(file =>
            {
                string name = Path.GetFileNameWithoutExtension(file.name);
                string[] parts = name.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int num))
                    return num;
                return -1;
            })
            .Where(num => num > 0) // skip 0
            .DefaultIfEmpty(0)
            .Max();

        // Wrap levelToLoad: if > maxLevel, start back at 1
        if (levelToLoad > maxLevel)
        {
            levelToLoad = ((levelToLoad - 1) % maxLevel) + 1;
        }

        string persistentLevelPath = Path.Combine(Application.persistentDataPath, "Levels", levelToLoad.ToString(), $"Level_{levelToLoad}.json");
        string json;

        if (File.Exists(persistentLevelPath))
        {
            json = File.ReadAllText(persistentLevelPath);
            Debug.Log("Loaded level data from persistent path.");
        }
        else
        {
            TextAsset jsonFile = Resources.Load<TextAsset>($"Levels/{levelToLoad}/Level_{levelToLoad}");
            if (jsonFile == null || seatPrefab == null)
            {
                Debug.LogError("Missing JSON file or Seat Prefab in inspector.");
                return;
            }

            json = jsonFile.text;

            // Ensure directory exists
            string dirPath = Path.GetDirectoryName(persistentLevelPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            File.WriteAllText(persistentLevelPath, json);
            Debug.Log("Created persistent level file from resources.");
        }

        GamePlayManager.Instance.SetLevel(levelToLoad);
        LoadLevelData(json);
        InitialiseLevel();
    }


    public void MakeSeatOpen(string seatID)
    {
        var seat = levelData.seats.FirstOrDefault(s => s.seatNumber == seatID);
        if (seat != null)
        {
            seat.isInitiallyOpened = true;

            // Save updated JSON back to persistent path
            string persistentLevelPath = Path.Combine(Application.persistentDataPath, "Levels", levelToLoad.ToString(), $"Level_{levelToLoad}.json");

            string updatedJson = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(persistentLevelPath, updatedJson);

            Debug.Log($"Seat {seatID} marked as open and saved to persistent file.");
        }
        else
        {
            Debug.LogWarning($"Seat ID {seatID} not found in level data.");
        }
    }

    public void SaveLevelData()
    {
        string json = JsonUtility.ToJson(levelData, true);
        string filePath = Path.Combine(Application.persistentDataPath, "Levels", levelToLoad.ToString(), $"Level_{levelToLoad}.json");

        // Create directories if they don't exist
        string dirPath = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        File.WriteAllText(filePath, json);
        Debug.Log($"Level data saved to {filePath}");
    }

    [ContextMenu("Delete Persistent Levels Path")]
    public void DeletePersistentLevelsPath()
    {
        string levelsPath = Path.Combine(Application.persistentDataPath, "Levels");

        if (Directory.Exists(levelsPath))
        {
            Directory.Delete(levelsPath, true);
            Debug.Log("Deleted entire persistent levels path.");
        }
        else
        {
            Debug.LogWarning("No persistent levels path found to delete.");
        }
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
        float offset = SafeAreaUtils.GetSafeAreaTopOffsetWorld(Camera.main, Screen.safeArea);


        var seatData = levelData.seatsGrid;
        var allPersons = new List<string>();
        var openPersons = new List<string>();
        List<Seat> seatBounds = new List<Seat>();

        foreach (var seat in seatData)
        {
            allPersons.Add(seat.personName.Trim());
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
                    allSeatComponents.Add(seatComponent);
                    var seatBound = seatComponent.GetBounds();
                    seatBounds.Add(seatComponent);

                    Vector2 spawnPos = new((seatBound.size.x + 0.1f) * y, -(seatBound.size.y + 0.1f) * x); // y = col, x = row
                    seatObj.transform.position = spawnPos;
                    seatPositionsMean += spawnPos;
                    PersonData personData = Resources.Load<PersonData>($"Levels/{levelToLoad}/{currentSeat.personName.Trim()}");

                    string hyperHintText = TextStyler.GiveHyperText(currentSeat.hint, allPersons);
                    seatComponent.LoadData(currentSeat.personName.Trim(), personData.gender, personData.personIcon, currentSeat.seatNumber, hyperHintText);
                    seatComponent.SetLinkedSeats(currentSeat.linkedSeats);
                    seatComponent.mySeatID = currentSeat.seatNumber;
                    seatComponent.makeTextClickable(allPersons);

                    personSeatIdsByName[currentSeat.personName] = currentSeat.seatNumber;
                    seatCompsByName[currentSeat.personName] = seatComponent;

                    if (!currentSeat.isInitiallyOpened)
                    {
                        SeatSelector.Instance.AddSeat(seatComponent);
                    }
                    else
                    {
                        openPersons.Add(currentSeat.personName.Trim());
                        seatComponent.isOpenSeat = true;
                        seatComponent.isPlaced = true;
                        placedSeats.Add(currentSeat.seatNumber);
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

        int minSeatsVisible = 6;

        if (seatData.GetLength(1) > minSeatsVisible)
        {
            seatsCenterPos.x = allSeats[0, 0].transform.position.x - (seatBounds[0].GetBounds().size.x / 2f);
        }

        seatsCentre.transform.position = seatsCenterPos;


        foreach (var item in allSeats)
        {
            item.SetParent(seatsCentre.transform);
        }
        if (seatData.GetLength(1) <= minSeatsVisible)
        {
            seatsCentre.transform.position = VptoWP(0.5f, 1f) - new Vector2(0, 2f) - new Vector2(0, offset);
        }
        else
        {
            seatsCentre.transform.position = VptoWP(0f, 1f) + new Vector2(0.25f, -2f) - new Vector2(0, offset);
        }

        for (int p = 0; p < levelData.seats.Count; p++)
        {
            var personName = levelData.seats[p].personName.Trim();
            PersonData personData = Resources.Load<PersonData>($"Levels/{levelToLoad}/{personName}");
            if (personData != null && !openPersons.Contains(personName))
            {
                var personItem = UIManager.Instance.SpawnNewPerson(personData.personIcon, personName, personData.gender);
                personItemsByIds[personSeatIdsByName[personName]] = personItem;
            }
        }

        CameraDragMove.Instance.AdjustBoundary(seatBounds);
        SeatSelector.Instance.InitialseSeats(openPersons.Count, allPersons.Count);

    }

    Vector2 VptoWP(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new(x, y, 0));
    }

    public void SeatPlaced(string seatID)
    {
        placedSeats.Add(seatID);
        CheckForSolvedSeats();
    }

    public void CheckForSolvedSeats()
    {
        foreach (var item in allSeatComponents)
        {
            bool solvedAllLinkedSeats = false;
            foreach (var seatID in item.linkedSeatIDs)
            {

                if (placedSeats.Contains(seatID))
                {
                    solvedAllLinkedSeats = true;

                }
                else
                {
                    solvedAllLinkedSeats = false;
                    break;
                }
            }

            if (solvedAllLinkedSeats && (item.isPlaced || item.isOpenSeat))
            {
                item.SeatSolved();
            }

        }
    }

    public Seat GetSeatFromName(string personName)
    {

        foreach (var seat in allSeatComponents)
        {
            if (seat.PersonName == personName)
            {
                return seat;
            }
        }

        return null;
    }

    public (Seat, PersonItem) SolveOneEligiblePersonItemsYetToBeSolved()
    {
        List<string> seatIds = new List<string>();

        foreach (var seat in allSeatComponents)
        {
            if (seat.isPlaced || seat.isOpenSeat)
            {

                seatIds.AddRange(seat.linkedSeatIDs);
            }
        }

        foreach (var seat in allSeatComponents)
        {
            if (!seat.isPlaced)
            {
                seatIds.Add(seat.mySeatID);
            }
        }

        seatIds = RemoveDuplicatesPreserveOrder(seatIds);

        List<PersonItem> eligiblePersonItems = new List<PersonItem>();

        foreach (var seatId in seatIds)
        {
            if (personItemsByIds.ContainsKey(seatId) && (personItemsByIds[seatId] != null))
            {
                eligiblePersonItems.Add(personItemsByIds[seatId]);
            }
        }

        if (eligiblePersonItems.Count > 0)
        {
            return (seatCompsByName[eligiblePersonItems[0].PersonName], eligiblePersonItems[0]);
        }
        else
        {
            Debug.Log("No eligible person items to solve.");
            return (null, null);
        }

    }

    List<string> RemoveDuplicatesPreserveOrder(List<string> list)
    {

        List<string> result = new List<string>();

        foreach (string item in list)
        {
            if (!result.Contains(item))
            {
                result.Add(item);
            }
        }

        return result;
    }

}
