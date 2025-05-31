using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class CreatePersonDataFromJson : Editor
{
    [MenuItem("Assets/Create Persons", false, 1)]
    private static void CreatePersonsFromJson()
    {
        string jsonPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        if (!File.Exists(jsonPath) || !jsonPath.EndsWith(".json"))
        {
            Debug.LogError("Please select a valid JSON file.");
            return;
        }

        string json = File.ReadAllText(jsonPath);
        string seatsArrayJson = ExtractSeatsArray(json);
        LevelDataWrapper wrapper = JsonUtility.FromJson<LevelDataWrapper>("{\"seats\":" + seatsArrayJson + "}");

        if (wrapper == null || wrapper.seats == null)
        {
            Debug.LogError("Failed to parse JSON.");
            return;
        }

        string assetFolder = Path.GetDirectoryName(jsonPath);

        if (!Directory.Exists(assetFolder))
            Directory.CreateDirectory(assetFolder);

        foreach (var seat in wrapper.seats)
        {
            if (string.IsNullOrEmpty(seat.personName))
                continue;

            PersonData personData = ScriptableObject.CreateInstance<PersonData>();
            personData.gender = seat.personGender == "Male" ? Gender.Male : Gender.Female;
            personData.LOADSPRITE();

            string fileName = $"{seat.personName}.asset";
            string assetPath = Path.Combine(assetFolder, fileName);

            AssetDatabase.CreateAsset(personData, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Created PersonData ScriptableObjects at: {assetFolder}");
    }

    // Utility method to extract the seats array
    private static string ExtractSeatsArray(string fullJson)
    {
        int start = fullJson.IndexOf("\"seats\":") + 8;
        int end = fullJson.LastIndexOf("]");
        return fullJson.Substring(start, end - start + 1);
    }

    [System.Serializable]
    private class LevelDataWrapper
    {
        public List<SeatData> seats;
    }

    [System.Serializable]
    public class SeatData
    {
        public string seatNumber;
        public int row;
        public int col;
        public string personName;
        public string personGender;
        public string hint;
        public bool isInitiallyOpened;
        public List<string> linkedSeats;
        public List<string> tags;
    }
}
