using TMPro;
using UnityEngine;
using DG.Tweening;

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

    string assignedToPerson;

    public Color correctColor;
    public Color normalColor;

    public GameObject happyEmoji;
    [SerializeField] Transform checkMark;


    public bool isOpenSeat;

    public string PersonName
    {
        get { return assignedToPerson; }
    }

    public Vector2 SeatingPos
    {
        get { return mySeatSR.transform.position; }
    }

    public void LoadData(string personName, Sprite personIcon, string seatNumber, string hint)
    {
        personSR.sprite = personIcon;
        assignedToPerson = personName;
        seatNumberIndicator.text = seatNumber;
        hintIndicator.text = hint;
        this.personName.text = personName;
    }



    // Make sure DOTween is imported

    public void CorrectPlacement()
    {
        personObject.SetActive(true);
        BGBottom.gameObject.SetActive(true);
        happyEmoji.SetActive(true);

        checkMark.DOScale(1f, 1f).SetDelay(.2f).SetEase(Ease.OutBounce);

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

        seatNumberIndicator.DOColor(correctColor, 0.5f).SetDelay(.2f).SetEase(Ease.InOutQuad);

        BGRed.color = correctColor;
        BGRed.DOKill();

        Color bgColor = BGRed.color;
        bgColor.a = 0;
        BGRed.color = bgColor;


        // Fade in and out BGRed
        BGRed.DOFade(.8f, 0.4f) // Fade in quickly
            .OnComplete(() =>
            {
                BGRed.DOFade(0f, 0.5f).SetDelay(.2f); // Then fade out more slowly
            });
    }

    public void SetOpenSeat()
    {
        checkMark.localScale = Vector3.one;
        seatNumberIndicator.color = correctColor;

        // Animate personObject scale from 0 to 1 with bounce
        personObject.transform.localScale = Vector3.zero;
        personObject.SetActive(true);
        personObject.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        // Move seatContainer to correct position
        seatContainer.DOLocalMove(seatCorrectPlacement, 0.5f).SetEase(Ease.OutCubic);

        BGBottom.gameObject.SetActive(true);

        // Fade in hintIndicator (TextMeshProUGUI)
        Color hintColor = hintIndicator.color;
        hintColor.a = 0f;
        hintIndicator.color = hintColor;
        hintIndicator.gameObject.SetActive(true);

        hintIndicator.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
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


}
