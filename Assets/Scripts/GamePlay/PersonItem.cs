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
    public Image personRefIcon;

    public void LoadData(string personName, Sprite personIcon)
    {
        this.personName = personName;
        this.personIcon.sprite = personIcon;
        this.personRefIcon.sprite = personIcon;
        personNameIndicator.text = personName;

    }

    public override void TargetReached()
    {
        if (assignedSeat.PersonName == personName)
        {
            assignedSeat.CorrectPlacement();
            SeatSelector.Instance.SeatPlaced(assignedSeat);
            Destroy(contentToDrag.gameObject);
            Destroy(personIconRef.gameObject);
            Destroy(gameObject);

            SoundManager.Play(SoundNames.Correct, .5f);
        }
        else
        {
            assignedSeat.WrongAnimation();
            BackToScrollPanel();
            //SoundManager.Play(SoundNames.Wrong);
            SoundManager.Play(SoundNames.Error);

        }
    }

    public Vector2 GetSP()
    {
        return Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(personIcon.transform.position));
    }



}
