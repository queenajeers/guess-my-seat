using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;


public class Seat : MonoBehaviour
{
    [SerializeField] SpriteRenderer mySeatSR;
    [SerializeField] SpriteRenderer personSR;

    [SerializeField] TextMeshPro seatNumberIndicator;
    [SerializeField] TextMeshPro hintIndicator;
    [SerializeField] TextMeshPro personName;

    [SerializeField] SpriteRenderer BGRed;

    [SerializeField] SpriteRenderer BGBottom;
    [SerializeField] SpriteRenderer Border;

    [SerializeField] Transform seatContainer;
    [SerializeField] GameObject personObject;

    [SerializeField] Vector2 seatCorrectPlacement;
    Vector2 personIconOriginalPosition;

    string assignedToPerson;
    public Gender gender;

    Color CorrectColor
    {
        get { return gender == Gender.Male ? correctColorMale : correctColorFemale; }

    }

    public Color correctColorMale;
    public Color correctColorFemale;
    public Color normalColor;
    public Color solvedHintColor;

    public GameObject happyEmoji;

    [SerializeField] Transform checkMark;
    [SerializeField] Transform checkMarkSolved;
    [SerializeField] Transform personIcon;

    [SerializeField] ClickableWordsHandler clickableWordsHandler;


    public bool isOpenSeat;
    public bool holdSeat;

    public bool isPlaced;
    public bool isSeatSolved;

    public List<string> linkedSeatIDs;
    public string mySeatID;


    public string PersonName
    {
        get { return assignedToPerson; }
    }

    public Vector2 SeatingPos
    {
        get { return mySeatSR.transform.position; }
    }

    public int Lives
    {
        get { return PlayerPrefs.GetInt("Lives", 2); }
    }

    void Start()
    {
        personIconOriginalPosition = personIcon.transform.localPosition;
        ClickableWordsHandler.OnWordClicked += MakeIconJump;
    }

    void OnDestroy()
    {
        ClickableWordsHandler.OnWordClicked -= MakeIconJump;
    }

    public void LoadData(string personName, Gender gender, Sprite personIcon, string seatNumber, string hint)
    {
        personSR.sprite = personIcon;
        assignedToPerson = personName;
        this.gender = gender;
        seatNumberIndicator.text = seatNumber;
        hintIndicator.text = hint;
        this.personName.text = personName;

        if (hint.Length == 0)
        {
            hintIndicator.text = "------\n------";
            hintIndicator.color = solvedHintColor;
        }
    }

    public void SetLinkedSeats(List<string> linkedSeats)
    {
        linkedSeatIDs.AddRange(linkedSeats);
    }

    public void makeTextClickable(List<string> words)
    {
        clickableWordsHandler.InitializeClickableWords(words);

    }

    // Make sure DOTween is imported

    public void SeatSolved()
    {
        if (!isSeatSolved)
        {
            isSeatSolved = true;
            hintIndicator.DOColor(solvedHintColor, 0.5f).SetDelay(.2f).SetEase(Ease.InOutQuad);
            // checkMarkSolved.transform.localRotation = Quaternion.Euler(0, 0, -185);
            // checkMarkSolved.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), .5f);
            // checkMarkSolved.DOScale(1f, .5f).SetDelay(.2f).SetEase(Ease.OutBack);
        }
    }

    public void CorrectPlacement()
    {
        isPlaced = true;

        LevelLoader.Instance.SeatPlaced(mySeatID);


        personObject.SetActive(true);
        BGBottom.gameObject.SetActive(true);
        happyEmoji.SetActive(true);
        checkMark.transform.localRotation = Quaternion.Euler(0, 0, -185);
        checkMark.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), .5f);
        checkMark.DOScale(1f, .5f).SetDelay(.2f).SetEase(Ease.OutBack);

        // Move seatContainer to seatCorrectPlacement
        seatContainer.DOLocalMove(seatCorrectPlacement, 0.35f).SetEase(Ease.OutQuad);

        // Make sure the hintIndicator is active and alpha is 0 before fading in
        hintIndicator.gameObject.SetActive(true);

        Color hintColor = hintIndicator.color;
        hintColor.a = 0;
        hintIndicator.color = hintColor;

        Color bgBottomColor = BGBottom.color;
        bgBottomColor.a = 0;
        BGBottom.color = bgBottomColor;


        // Fade in the hint text
        hintIndicator.DOFade(1f, 0.5f).SetDelay(.2f).SetEase(Ease.InOutQuad);
        BGBottom.DOFade(1f, 0.5f).SetDelay(.2f).SetEase(Ease.InOutQuad);

        seatNumberIndicator.DOColor(CorrectColor, 0.5f).SetDelay(.2f).SetEase(Ease.InOutQuad);

        BGRed.color = CorrectColor;
        BGRed.DOKill();

        Color bgColor = BGRed.color;
        bgColor.a = 0;
        BGRed.color = bgColor;


        // Fade in and out BGRed
        BGRed.DOFade(1f, 0.4f); // Fade in quickly
                                // .OnComplete(() =>
                                // {
                                //     BGRed.DOFade(0f, 0.5f).SetDelay(.2f); // Then fade out more slowly
                                // });

        LevelLoader.Instance.MakeSeatOpen(mySeatID);

        UIManager.Instance.SayComplement();
    }

    public void SetOpenSeatOld()
    {
        checkMark.transform.localRotation = Quaternion.Euler(0, 0, -185);
        checkMark.DORotateQuaternion(Quaternion.Euler(0, 0, 0), .5f);
        checkMark.DOScale(1f, .5f).SetDelay(.2f).SetEase(Ease.OutBack);
        seatNumberIndicator.color = CorrectColor;

        personObject.SetActive(true);

        seatContainer.DOLocalMove(seatCorrectPlacement, 0.5f).SetEase(Ease.OutCubic);

        BGBottom.gameObject.SetActive(true);

        Color hintColor = hintIndicator.color;
        hintColor.a = 0f;
        hintIndicator.color = hintColor;
        hintIndicator.gameObject.SetActive(true);

        hintIndicator.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);

        BGRed.color = CorrectColor;
    }

    public void SetOpenSeat()
    {
        checkMark.transform.localRotation = Quaternion.Euler(0, 0, -185);
        checkMark.localRotation = Quaternion.Euler(0, 0, 0);
        checkMark.localScale = Vector3.one;

        seatNumberIndicator.color = CorrectColor;

        personObject.SetActive(true);

        seatContainer.localPosition = seatCorrectPlacement;

        BGBottom.gameObject.SetActive(true);

        hintIndicator.gameObject.SetActive(true);

        BGRed.color = CorrectColor;
    }

    public void PopUp()
    {
        var currentScale = transform.localScale;

        transform.DOScale(currentScale * 1.06f, .2f).OnComplete(() =>
        {
            transform.DOScale(currentScale, .4f).SetEase(Ease.OutBack);
        });
    }

    public void WrongAnimation()
    {
        BGRed.DOKill();

        // Fade in and out BGRed
        BGRed.DOFade(.8f, 0.4f) // Fade in quickly
             .OnComplete(() =>
             {
                 BGRed.DOFade(0f, 0.5f).SetDelay(.2f); // Then fade out more slowly
             });

        mySeatSR.DOKill();
        // Shake the seat horizontally
        mySeatSR.transform.DOShakePosition(
            duration: 0.5f,
            strength: new Vector3(0.3f, 0, 0), // Only shake on X-axis
            vibrato: 10,
            randomness: 0,
            snapping: false,
            fadeOut: true
        );
    }

    public Bounds GetBounds()
    {
        return BGRed.bounds;
    }

    public Vector2 GetSP()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    public void MakeIconJump(string personName)
    {
        if (personName.Contains(assignedToPerson) && (personName.Length == assignedToPerson.Length))
        {
            // Kill any existing tweens on the icon
            personIcon.transform.DOKill();
            personIcon.transform.localPosition = personIconOriginalPosition;
            // Create a jump sequence
            personIcon.transform.DOLocalMove(personIconOriginalPosition + new Vector2(0, .5f), .2f).OnComplete(() =>
            {
                personIcon.transform.DOLocalMove(personIconOriginalPosition, .2f).SetEase(Ease.OutBounce);
            });
        }

    }




}
