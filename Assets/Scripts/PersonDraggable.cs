using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PersonDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Target Settings")]
    public RectTransform contentToDrag;
    public Vector2 dragOffset = new Vector2(0f, 50f);
    public float dragThreshold = 10f;
    public float smoothFollowSpeed = 10f;
    public float returnDuration = 0.25f;

    private bool isVerticalDrag = false;
    private bool wasDragged = false;
    private Vector2 initialPointerPosition;

    private Canvas rootCanvas;

    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 originalLocalPosition;
    private Vector2 originalSizeDelta;

    private Coroutine smoothFollowCoroutine;
    private Coroutine returnCoroutine;

    private bool targetFound = false;
    private Vector3 targetWorldPosition;

    private void Awake()
    {
        if (contentToDrag == null)
        {
            Transform found = transform.Find("Content");
            if (found != null)
                contentToDrag = found.GetComponent<RectTransform>();
        }

        rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        // Only if this object was dragged
        if (wasDragged && Input.GetKeyDown(KeyCode.Space))
        {
            if (returnCoroutine != null)
                StopCoroutine(returnCoroutine);

            returnCoroutine = StartCoroutine(SmoothReturn());
            targetFound = false;
            wasDragged = false;
        }

        if (wasDragged && Input.GetKeyDown(KeyCode.T))
        {
            SetTargetStatus(true, Vector2.zero);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isVerticalDrag = false;
        wasDragged = false;
        initialPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - initialPointerPosition;

        if (!isVerticalDrag && delta.magnitude > dragThreshold)
        {
            isVerticalDrag = Mathf.Abs(delta.y) > Mathf.Abs(delta.x);

            if (isVerticalDrag && contentToDrag != null)
            {
                wasDragged = true;

                originalParent = contentToDrag.parent;
                originalSiblingIndex = contentToDrag.GetSiblingIndex();
                originalLocalPosition = contentToDrag.localPosition;
                originalSizeDelta = contentToDrag.sizeDelta;

                Vector3 worldPos = contentToDrag.position;
                Vector2 size = contentToDrag.sizeDelta;

                contentToDrag.SetParent(UIManager.Instance.GamePlayPanel, worldPositionStays: false);
                contentToDrag.position = worldPos;
                contentToDrag.sizeDelta = size;
            }
        }

        if (isVerticalDrag && contentToDrag != null)
        {
            Vector3 targetWorldPos;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                rootCanvas.transform as RectTransform,
                eventData.position + dragOffset,
                eventData.pressEventCamera,
                out targetWorldPos
            );

            if (smoothFollowCoroutine != null)
                StopCoroutine(smoothFollowCoroutine);

            smoothFollowCoroutine = StartCoroutine(SmoothFollow(targetWorldPos));
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isVerticalDrag || contentToDrag == null) return;

        if (smoothFollowCoroutine != null)
            StopCoroutine(smoothFollowCoroutine);

        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        if (targetFound)
        {
            returnCoroutine = StartCoroutine(SmoothMoveToTarget(targetWorldPosition));
        }
        else
        {
            returnCoroutine = StartCoroutine(SmoothReturn());
        }

        isVerticalDrag = false;

    }

    private IEnumerator SmoothFollow(Vector3 targetWorldPos)
    {
        while (true)
        {
            if (contentToDrag == null) yield break;

            contentToDrag.position = Vector3.Lerp(contentToDrag.position, targetWorldPos, Time.deltaTime * smoothFollowSpeed);
            yield return null;
        }
    }

    private IEnumerator SmoothReturn()
    {
        Vector3 startPos = contentToDrag.position;
        Vector3 endPos = originalParent.TransformPoint(originalLocalPosition);

        float elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            contentToDrag.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        contentToDrag.SetParent(originalParent, worldPositionStays: false);
        contentToDrag.SetSiblingIndex(originalSiblingIndex);
        contentToDrag.localPosition = originalLocalPosition;
        contentToDrag.sizeDelta = originalSizeDelta;
        wasDragged = false;
    }

    private IEnumerator SmoothMoveToTarget(Vector3 worldTarget)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldTarget);
        Vector2 localPoint;

        RectTransform parentRect = contentToDrag.parent as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPoint, null, out localPoint);

        Vector3 startPos = contentToDrag.localPosition;
        Vector3 endPos = new Vector3(localPoint.x, localPoint.y, startPos.z);

        float elapsed = 0f;
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / returnDuration);
            contentToDrag.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        contentToDrag.localPosition = endPos;
    }

    public void SetTargetStatus(bool found, Vector3 worldPosition)
    {
        targetFound = found;
        targetWorldPosition = worldPosition;
    }
}
