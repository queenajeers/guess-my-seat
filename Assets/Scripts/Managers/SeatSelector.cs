using System.Collections.Generic;
using UnityEngine;

public class SeatSelector : MonoBehaviour
{
    public static SeatSelector Instance { get; private set; }

    [SerializeField] List<Seat> availableSeats = new List<Seat>();
    [SerializeField] float minDistForSeatPlacement = 1.5f; // Set in Inspector or tweak

    PersonDraggable currentPersonDraggable;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (currentPersonDraggable == null) return;

        Seat nearestSeat = null;
        float nearestDistance = Mathf.Infinity;

        foreach (var seat in availableSeats)
        {
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

    public void OnVerifySeatPlacement(Vector2 placementPos)
    {
        // Find the closest seat to where the person was dropped
        Seat seatToAssign = null;
        float closestDist = Mathf.Infinity;

        foreach (var seat in availableSeats)
        {
            float dist = Vector2.Distance(seat.SeatingPos, placementPos);
            if (dist < closestDist)
            {
                closestDist = dist;
                seatToAssign = seat;
            }
        }

        if (seatToAssign != null && closestDist <= minDistForSeatPlacement && false)
        {
            availableSeats.Remove(seatToAssign);
            // You might also want to do something like: seatToAssign.AssignPerson(currentPersonDraggable);
        }
        else
        {
            currentPersonDraggable.BackToScrollPanel();
        }

        currentPersonDraggable = null;
    }
}
