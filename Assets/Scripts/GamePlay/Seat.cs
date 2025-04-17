using TMPro;
using UnityEngine;
using DG.Tweening;

public class Seat : MonoBehaviour
{
    [SerializeField] SpriteRenderer mySeatSR;
    [SerializeField] TextMeshPro seatNumberIndicator;
    [SerializeField] TextMeshPro hintIndicator;
    [SerializeField] TextMeshPro personName;

    [SerializeField] SpriteRenderer BGRed;

    [SerializeField] Transform seatContainer;
    [SerializeField] GameObject personObject;

    [SerializeField] Vector2 seatCorrectPlacement;

    string assignedToPerson;

    public string PersonName
    {
        get { return assignedToPerson; }
    }

    public Vector2 SeatingPos
    {
        get { return mySeatSR.transform.position; }
    }

    public void LoadData(string personName, string seatNumber, string hint)
    {
        assignedToPerson = personName;
        seatNumberIndicator.text = seatNumber;
        hintIndicator.text = hint;
        this.personName.text = personName;
    }

    // Make sure DOTween is imported

    public void CorrectPlacement()
    {
        personObject.SetActive(true);

        // Move seatContainer to seatCorrectPlacement
        seatContainer.DOLocalMove(seatCorrectPlacement, 0.5f).SetEase(Ease.OutQuad);

        // Make sure the hintIndicator is active and alpha is 0 before fading in
        hintIndicator.gameObject.SetActive(true);

        Color hintColor = hintIndicator.color;
        hintColor.a = 0;
        hintIndicator.color = hintColor;

        // Fade in the hint text
        hintIndicator.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad);
    }


    public void WrongAnimation()
    {
        BGRed.DOKill();

        // Fade in and out BGRed
        BGRed.DOFade(.5f, 0.2f) // Fade in quickly
            .OnComplete(() =>
            {
                BGRed.DOFade(0f, 0.5f); // Then fade out more slowly
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


}
