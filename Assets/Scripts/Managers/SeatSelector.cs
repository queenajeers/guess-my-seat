using System.Collections.Generic;
using UnityEngine;

public class SeatSelector : MonoBehaviour
{
    public static SeatSelector Instance { get; private set; }

    [SerializeField] List<Seat> availableSeats = new List<Seat>();
    [SerializeField] float minDistForSeatPlacement = 1.5f; // Set in Inspector or tweak

    PersonDraggable currentPersonDraggable;

    public int seatsFilled;
    public int totalSeats;

    void Awake()
    {
        Instance = this;
    }

    public void AddSeat(Seat seat)
    {
        availableSeats.Add(seat);
    }

    void Update()
    {
        if (currentPersonDraggable == null) return;

        Seat nearestSeat = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var seat in availableSeats)
        {
            if (seat.holdSeat) continue;
            float dist = Vector2.Distance(currentPersonDraggable.ContentRefWorldPos, seat.SeatingPos);

            if (dist < nearestDistance)
            {
                nearestDistance = dist;
                nearestSeat = seat;
            }
        }

        if (nearestSeat != null && nearestDistance <= minDistForSeatPlacement)
        {

            currentPersonDraggable.assignedSeat = nearestSeat;
            currentPersonDraggable.SetTargetStatus(true, nearestSeat.SeatingPos);
        }
        else
        {
            currentPersonDraggable.assignedSeat = null;
            currentPersonDraggable.SetTargetStatus(false, Vector2.zero);
        }

    }

    public void SelectedCurrentDraggable(PersonDraggable personDraggable)
    {
        currentPersonDraggable = personDraggable;
    }

    public void CancelDraggable(PersonDraggable personDraggable)
    {
        if (currentPersonDraggable == personDraggable)
        {
            currentPersonDraggable = null;
        }
    }

    public void InitialseSeats(int seatsFilled, int totalSeats)
    {
        this.seatsFilled = seatsFilled;
        this.totalSeats = totalSeats;

        UIManager.Instance.SetSeatsIndicator(seatsFilled, totalSeats);

    }

    public void SeatPlaced(Seat seat)
    {
        if (availableSeats.Contains(seat))
        {
            availableSeats.Remove(seat);
            seatsFilled++;
            UIManager.Instance.SetSeatsIndicator(seatsFilled, totalSeats);

            if (availableSeats.Count == 0)
            {
                Debug.Log("LEVEL FINISHED!");
                UIManager.Instance.FinishActivate();
                UIManager.Instance.GamePlayElementsOut();
                LevelLoader.Instance.DeletePersistentLevelsPath();

            }

        }
    }


}
