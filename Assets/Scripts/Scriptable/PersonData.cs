using UnityEngine;

public enum Gender
{
    Male,
    Female
}

[CreateAssetMenu(fileName = "NewPersonData", menuName = "ScriptableObjects/PersonData", order = 1)]
public class PersonData : ScriptableObject
{

    public string personName;
    public Gender gender;
    public Sprite personIcon;

}
