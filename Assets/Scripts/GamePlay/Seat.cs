using TMPro;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [SerializeField] SpriteRenderer mySeatSR;
    [SerializeField] TextMeshPro seatNumberIndicator;
    [SerializeField] TextMeshPro hintIndicator;

    string assignedToPerson;

    public Vector2 SeatingPos
    {
        get { return mySeatSR.transform.position; }
    }

    public void LoadData(string personName, string seatNumber, string hint)
    {
        assignedToPerson = personName;
        seatNumberIndicator.text = seatNumber;
        hintIndicator.text = hint;
    }

}
