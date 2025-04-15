using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalOnlyWhenGestureIsHorizontal : ScrollRect
{
    private bool isHorizontalDrag;
    private bool directionDecided = false;
    private Vector2 initialDragPosition;

    public float directionThreshold = 0.1f; // How much drag movement before we decide the direction

    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        base.OnInitializePotentialDrag(eventData);
        directionDecided = false;
        isHorizontalDrag = false;
        initialDragPosition = eventData.position;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsActive()) return;

        Vector2 delta = eventData.position - initialDragPosition;

        if (!directionDecided)
        {
            // Only decide when drag is significant enough
            if (delta.magnitude > directionThreshold)
            {
                directionDecided = true;
                isHorizontalDrag = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);
            }
            else
            {
                return; // Don't start drag yet
            }
        }

        if (isHorizontalDrag)
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (isHorizontalDrag)
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (isHorizontalDrag)
        {
            base.OnEndDrag(eventData);
        }

        // Reset after drag ends
        directionDecided = false;
        isHorizontalDrag = false;
    }
}
