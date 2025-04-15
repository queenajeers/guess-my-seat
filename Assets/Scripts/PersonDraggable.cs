using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PersonDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Target Settings")]
    public RectTransform contentToDrag;
    public Vector2 dragOffset = new Vector2(0f, 50f);
    public float dragThreshold = 10f;
    public float smoothFollowSpeed = 10f;
    public float returnDuration = 0.25f;

    [Header("Scroll Settings")]
    public bool enableHorizontalScrollTransfer = true;
    public ScrollRect parentScrollRect;
    public float horizontalScrollSensitivity = 1f;
    public float horizontalScrollVelocityMultiplier = 2f; // Controls inertia strength

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

    private Coroutine smoothFollowCoroutine;
    private Coroutine returnCoroutine;
    private Coroutine scrollInertiaCoroutine;

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

        // Find parent ScrollRect if not assigned
        if (parentScrollRect == null)
        {
            parentScrollRect = GetComponentInParent<ScrollRect>();
        }
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
        isDragging = true;
        isVerticalDrag = false;
        isHorizontalDrag = false;
        wasDragged = false;
        initialPointerPosition = eventData.position;
        lastPointerPosition = eventData.position;
        lastDragTime = Time.unscaledTime;
        pointerDeltaTime = 0;

        // Stop any ongoing inertia
        if (scrollInertiaCoroutine != null)
        {
            StopCoroutine(scrollInertiaCoroutine);
            scrollInertiaCoroutine = null;
        }

        // Cache the current ScrollRect state, but don't disable it yet
        if (parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            // Keep scrolling enabled but capture velocity if there is any
            scrollVelocity = parentScrollRect.velocity;
            parentScrollRect.StopMovement(); // Stop any ongoing movement
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - initialPointerPosition;
        Vector2 frameDelta = eventData.position - lastPointerPosition;

        // Calculate time since last update for velocity calculation
        float currentTime = Time.unscaledTime;
        pointerDeltaTime = Mathf.Max(currentTime - lastDragTime, 0.001f); // Avoid division by zero
        lastDragTime = currentTime;

        // Store last position for next frame
        lastPointerPosition = eventData.position;

        // Determine drag direction after crossing threshold
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

                // Make sure the ScrollRect is set to horizontal scrolling
                if (parentScrollRect != null && enableHorizontalScrollTransfer)
                {
                    parentScrollRect.horizontal = true;
                }
            }
        }

        // Handle horizontal scrolling
        if (isHorizontalDrag && parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            // Calculate horizontal velocity for inertia
            scrollVelocity.x = frameDelta.x / pointerDeltaTime;

            // Apply the drag movement to the scroll rect
            float normalizedDelta = frameDelta.x / Screen.width * horizontalScrollSensitivity;
            parentScrollRect.horizontalNormalizedPosition -= normalizedDelta;
            return;
        }

        // Handle vertical dragging
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
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        // Handle horizontal scroll inertia
        if (isHorizontalDrag && parentScrollRect != null && enableHorizontalScrollTransfer)
        {
            // Apply velocity for elastic/inertia behavior
            Vector2 finalVelocity = scrollVelocity * horizontalScrollVelocityMultiplier;

            // Limit maximum velocity to prevent extreme scrolling
            finalVelocity.x = Mathf.Clamp(finalVelocity.x, -3000f, 3000f);

            // Only apply inertia if there's significant velocity
            if (Mathf.Abs(finalVelocity.x) > 50f)
            {
                parentScrollRect.velocity = finalVelocity;

                // Optional: Start a coroutine to handle additional custom inertia effects
                if (scrollInertiaCoroutine != null)
                    StopCoroutine(scrollInertiaCoroutine);

                scrollInertiaCoroutine = StartCoroutine(HandleScrollInertia());
            }
        }

        // Handle vertical drag ending
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
        }

        isVerticalDrag = false;
        isHorizontalDrag = false;
    }

    private IEnumerator HandleScrollInertia()
    {
        // Optional: Add custom inertia handling here
        // For example, you could modify the ScrollRect's elasticity behavior

        // Wait for the ScrollRect to finish its elastic movement
        float startTime = Time.unscaledTime;
        while (Time.unscaledTime - startTime < 0.5f &&
               (Mathf.Abs(parentScrollRect.velocity.x) > 0.1f ||
                Mathf.Abs(parentScrollRect.velocity.y) > 0.1f))
        {
            // Let Unity's built-in elasticity handle it
            yield return null;
        }

        // Ensure the content is within bounds after inertia settles
        yield return new WaitForSeconds(0.1f);

        // Check if we need to snap back if scrolled past bounds
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