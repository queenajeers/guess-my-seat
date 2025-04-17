using UnityEngine;

[CreateAssetMenu(fileName = "NewPersonData", menuName = "ScriptableObjects/PersonData", order = 1)]
public class PersonData : ScriptableObject
{

    public string personName;
    public Sprite personIcon;
}
