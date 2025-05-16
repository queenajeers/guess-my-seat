using DG.Tweening;
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

    Vector2 personIconOriginalPosition;

    public override void Start()
    {
        base.Start();
        personIconOriginalPosition = personIcon.transform.localPosition;
    }
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

    public void SolveIt()
    {
        Seat targetSeat = LevelLoader.Instance.GetSeatFromName(personName);

        if (targetSeat == null) return;

        targetSeat.CorrectPlacement();
        SeatSelector.Instance.SeatPlaced(targetSeat);
        Destroy(contentToDrag.gameObject);
        Destroy(personIconRef.gameObject);
        Destroy(gameObject);
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

            SoundManager.Play(SoundNames.Error);

            UIManager.Instance.LoseLife();

            if (DailyStatsDataManager.Instance != null)
            {
                DailyStatsDataManager.Instance.WrongChoice();
            }

        }
    }

    public void MakeIconJump()
    {
        // Kill any existing tweens on the icon
        personIcon.transform.DOKill();
        personIcon.transform.localPosition = personIconOriginalPosition;
        // Create a jump sequence
        personIcon.transform.DOLocalMove(personIconOriginalPosition + new Vector2(0, 30f), .2f).OnComplete(() =>
        {
            personIcon.transform.DOLocalMove(personIconOriginalPosition, .2f).SetEase(Ease.OutBounce);
        });

    }

    public Vector2 GetSP()
    {
        return Camera.main.WorldToScreenPoint(Camera.main.ScreenToWorldPoint(personIcon.transform.position));
    }



}
