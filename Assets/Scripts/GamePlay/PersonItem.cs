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

    public GameObject maleIcon;
    public GameObject femaleIcon;

    public void LoadData(string personName, Gender gender, Sprite personIcon)
    {
        this.personName = personName;
        this.personIcon.sprite = personIcon;
        this.personRefIcon.sprite = personIcon;
        personNameIndicator.text = personName;

        if (gender == Gender.Male)
        {
            maleIcon.SetActive(true);
        }
        else
        {
            femaleIcon.SetActive(true);
        }

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

    public void MakeIconJump()
    {

    }

    public Vector2 GetSP()
    {
        return Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(personIcon.transform.position));
    }



}
