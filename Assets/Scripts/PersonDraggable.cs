using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PersonDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Target Settings")]
    public RectTransform contentToDrag;
    public RectTransform personIconRef;
    public Vector2 dragOffset = new Vector2(0f, 50f);
    public float dragThreshold = 10f;
    public float smoothFollowSpeed = 10f;
    public float returnDuration = 0.25f;

    [Header("Scroll Settings")]
    public bool enableHorizontalScrollTransfer = true;
    public ScrollRect parentScrollRect;
    public float horizontalScrollSensitivity = 1f;
    public float horizontalScrollVelocityMultiplier = 2f;

    private bool isDragging = false;
    private bool isVerticalDrag = false;
    private bool isHorizontalDrag = false;
    private bool wasDragged = false;
    private Vector2 initialPointerPosition;
    private Vector2 lastPointerPosition;
    private float pointerDeltaTime;
    private Vector2 scrollVelocity;
    private float lastDragTime;

    private Canvas rootCanvas;

    private Transform originalParent;
    private int originalSiblingIndex;
    private Vector3 originalLocalPosition;
    private Vector2 originalSizeDelta;

    private Transform iconOriginalParent;
    private int iconOriginalSiblingIndex;
    private Vector3 iconOriginalLocalPosition;
    private Vector2 iconOriginalSizeDelta;

    private Coroutine smoothFollowCoroutine;
    private Coroutine returnCoroutine;
    private Coroutine scrollInertiaCoroutine;

    private bool targetFound = false;
    private Vector3 targetWorldPosition;

    public Seat assignedSeat;
    private Vector3 originalScale;
    private Vector3 iconOriginalScale;
    [SerializeField] float sizeMultiplier; // applied for contentToDrag and personIconRef when in dragging mode, returning to original again back to scale 1
    // Get sizeMultipler when ever needed from  MatchWidth.Instance.GetCurrentSizeRatio()
    public Vector2 ContentRefWorldPos
    {


        get { return Camera.main.ScreenToWorldPoint(contentToDrag.position); }

    }

    private void Awake()
    {

        if (contentToDrag == null)
        {
            Transform found = transform.Find("Content");
            if (found != null)
                contentToDrag = found.GetComponent<RectTransform>();
        }

        rootCanvas = GetComponentInParent<Canvas>();

        if (parentScrollRect == null)
        {
            parentScrollRect = GetComponentInParent<ScrollRect>();
        }

        if (personIconRef != null)
            personIconRef.gameObject.SetActive(false);
    }

    void Start()
    {
        SetPersonIconRefOriginals();
    }

    void SetPersonIconRefOriginals()
    {
        iconOriginalParent = personIconRef.parent;
        iconOriginalSiblingIndex = personIconRef.GetSiblingIndex();
        iconOriginalLocalPosition = personIconRef.localPosition;
        iconOriginalSizeDelta = personIconRef.sizeDelta;
    }

    public void BackToScrollPanel()
    {
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(SmoothReturn());
        targetFound = false;
        wasDragged = false;
        assignedSeat = null;

        ReturnPersonIcon();
    }

    private void Update()
    {
        if (wasDragged && Input.GetKeyDown(KeyCode.Space))
        {
            BackToScrollPanel();
        }

        if (wasDragged && Input.GetKeyDown(KeyCode.T))
        {
            SetTargetStatus(true, Vector2.zero);
        }

        if (wasDragged && Input.GetKeyDown(KeyCode.U))
        {
            SetTargetStatus(false, Vector2.zero);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {


        isDragging = true;
        isVerticalDrag = false;
        isHorizontalDrag = false;
        wasDragged = false;
        initialPointerPosition = eventData.position;
        lastPointerPosition = eventData.position;
        lastDragTime = Time.unscaledTime;
        pointerDeltaTime = 0;

        SeatSelector.Instance.SelectedCurrentDraggable(this); // 👈 ADD THIS

        if (scrollInertiaCoroutine != null)
        {
            StopCoroutine(scrollInertiaCoroutine);
            scrollInertiaCoroutine = null;
        }

        if (parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            scrollVelocity = parentScrollRect.velocity;
            parentScrollRect.StopMovement();
        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - initialPointerPosition;
        Vector2 frameDelta = eventData.position - lastPointerPosition;

        float currentTime = Time.unscaledTime;
        pointerDeltaTime = Mathf.Max(currentTime - lastDragTime, 0.001f);
        lastDragTime = currentTime;
        lastPointerPosition = eventData.position;

        if (!isVerticalDrag && !isHorizontalDrag && delta.magnitude > dragThreshold)
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
            {
                isVerticalDrag = true;
                PrepareForDrag();
            }
            else
            {
                isHorizontalDrag = true;

                if (parentScrollRect != null && enableHorizontalScrollTransfer)
                {
                    parentScrollRect.horizontal = true;
                }
            }
        }

        if (isHorizontalDrag && parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            scrollVelocity.x = frameDelta.x / pointerDeltaTime;

            float normalizedDelta = frameDelta.x / Screen.width * horizontalScrollSensitivity;
            parentScrollRect.horizontalNormalizedPosition -= normalizedDelta;
            return;
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

    private void PrepareForDrag()
    {
        if (contentToDrag != null)
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



            // Apply scaling based on sizeMultiplier from MatchWidth
            sizeMultiplier = MatchWidth.Instance.GetCurrentSizeRatio();
            originalScale = contentToDrag.localScale;
            contentToDrag.localScale = Vector3.one * sizeMultiplier;

            SoundManager.Play(SoundNames.Pick);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        SeatSelector.Instance.CancelDraggable(this); // 👈 ADD THIS

        if (isHorizontalDrag && parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            Vector2 finalVelocity = scrollVelocity * horizontalScrollVelocityMultiplier;
            finalVelocity.x = Mathf.Clamp(finalVelocity.x, -3000f, 3000f);

            if (Mathf.Abs(finalVelocity.x) > 50f)
            {
                parentScrollRect.velocity = finalVelocity;

                if (scrollInertiaCoroutine != null)
                    StopCoroutine(scrollInertiaCoroutine);

                scrollInertiaCoroutine = StartCoroutine(HandleScrollInertia());
            }
        }

        if (isVerticalDrag && contentToDrag != null)
        {
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

            if (personIconRef != null && personIconRef.gameObject.activeSelf)
            {
                ReturnPersonIcon();
            }
        }
        else
        {
            personIconRef.gameObject.SetActive(false);
        }

        isVerticalDrag = false;
        isHorizontalDrag = false;
    }


    private IEnumerator HandleScrollInertia()
    {
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < 0.5f &&
               (Mathf.Abs(parentScrollRect.velocity.x) > 0.1f ||
                Mathf.Abs(parentScrollRect.velocity.y) > 0.1f))
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        float normalizedPos = parentScrollRect.horizontalNormalizedPosition;
        if (normalizedPos < 0)
        {
            StartCoroutine(SnapScrollPosition(normalizedPos, 0));
        }
        else if (normalizedPos > 1)
        {
            StartCoroutine(SnapScrollPosition(normalizedPos, 1));
        }
    }

    private IEnumerator SnapScrollPosition(float from, float to)
    {
        float duration = 0.2f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            parentScrollRect.horizontalNormalizedPosition = Mathf.Lerp(from, to, EaseOutCubic(t));
            yield return null;
        }

        parentScrollRect.horizontalNormalizedPosition = to;
    }

    private float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
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

        // Return scale back to 1
        contentToDrag.localScale = originalScale;

        if (personIconRef != null)
            personIconRef.localScale = iconOriginalScale;

        ReturnPersonIcon();
        SoundManager.Play(SoundNames.Pick, 1f, 1.4f);
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
        if (assignedSeat != null)
        {
            TargetReached();
        }
    }

    public virtual void TargetReached()
    {

    }

    public void SetTargetStatus(bool found, Vector3 worldPosition)
    {
        targetFound = found;
        targetWorldPosition = worldPosition;

        if (personIconRef == null) return;
        if (!isVerticalDrag) return;

        if (found)
        {
            personIconRef.gameObject.SetActive(true);

            sizeMultiplier = MatchWidth.Instance.GetCurrentSizeRatio();
            iconOriginalScale = personIconRef.localScale;
            personIconRef.localScale = Vector3.one * sizeMultiplier;

            MoveToWorldSpace(personIconRef, targetWorldPosition, UIManager.Instance.GamePlayPanel);

            // 👇 Make sure personIconRef is rendered behind the contentToDrag
            personIconRef.SetSiblingIndex(0); // or any lower index than contentToDrag
        }
        else
        {
            ReturnPersonIcon();
        }
    }

    private void MoveToWorldSpace(RectTransform element, Vector3 targetWorldPosition, Transform newParent)
    {
        Vector2 size = element.sizeDelta;

        element.SetParent(newParent, worldPositionStays: false);

        element.sizeDelta = size;

        element.position = Camera.main.WorldToScreenPoint(targetWorldPosition);
    }

    private void ReturnPersonIcon()
    {
        if (personIconRef == null || iconOriginalParent == null) return;

        personIconRef.localScale = iconOriginalScale;

        personIconRef.SetParent(iconOriginalParent, worldPositionStays: false);
        personIconRef.SetSiblingIndex(iconOriginalSiblingIndex);
        personIconRef.localPosition = iconOriginalLocalPosition;
        personIconRef.sizeDelta = iconOriginalSizeDelta;
        personIconRef.gameObject.SetActive(false);
    }
}
