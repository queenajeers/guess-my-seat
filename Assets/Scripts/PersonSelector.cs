using UnityEngine;

public class PersonSelector : MonoBehaviour
{
    public static PersonSelector Instance { get; private set; }
    public PersonItem currentSelectedPerson;


    void Awake()
    {
        Instance = this;
    }

    public void SelectThis(PersonItem personItem)
    {
        if (currentSelectedPerson != null)
        {
            currentSelectedPerson.Neutral();
        }

        personItem.Select();
        currentSelectedPerson = personItem;
    }


}
