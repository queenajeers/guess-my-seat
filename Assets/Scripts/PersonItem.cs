using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class PersonItem : PersonDraggable
{
    [Space(20)]

    [Header("Person Data")]
    [SerializeField] string personName;
    public string PersonName
    {
        get { return personName; }
    }
    public TextMeshProUGUI personNameIndicator;
    public Image personIcon;

    public void LoadData(string personName, Sprite personIcon)
    {
        this.personName = personName;
        this.personIcon.sprite = personIcon;
        personNameIndicator.text = personName;

    }

    public override void TargetReached()
    {
        if (assignedSeat.PersonName == personName)
        {
            assignedSeat.CorrectPlacement();
            Destroy(contentToDrag.gameObject);
            Destroy(personIconRef.gameObject);
        }
        else
        {
            assignedSeat.WrongAnimation();
            BackToScrollPanel();
        }
    }



}
