using UnityEngine;

public enum Gender
{
    Male,
    Female
}

[CreateAssetMenu(fileName = "NewPersonData", menuName = "ScriptableObjects/PersonData", order = 1)]
public class PersonData : ScriptableObject
{
    public Gender gender;
    public Sprite personIcon;

    public void LOADSPRITE()
    {
        string path = gender == Gender.Male
            ? "People/images/adult/male"
            : "People/images/adult/female";

        Sprite[] sprites = Resources.LoadAll<Sprite>(path);

        if (sprites.Length > 0)
        {
            personIcon = sprites[Random.Range(0, sprites.Length)];
        }
        else
        {
            Debug.LogWarning("No sprites found at: " + path);
        }
    }
}
