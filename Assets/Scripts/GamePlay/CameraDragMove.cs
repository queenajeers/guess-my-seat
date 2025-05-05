using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

enum GestureMode { None, Pan, Zoom }


[RequireComponent(typeof(Camera))]
public class CameraDragMove : MonoBehaviour
{
    GestureMode currentGesture = GestureMode.None;
    public static CameraDragMove Instance { get; private set; }

    [Header("Drag Settings")]
    public float dragSpeed = 1f;

    [Header("Movement Boundaries")]
    public Vector2 minBounds = new Vector2(-10, -10);
    public Vector2 maxBounds = new Vector2(10, 10);

    [Header("Zoom Settings")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeedTouch = 0.1f;
    public float zoomSpeedMouse = 5f;
    public float zoomLerpSpeed = 10f; // Controls smoothness

    public float boundaryPadding = 1f;

    private Vector2 lastPanPosition;
    private bool isDragging = false;
    private bool startedOverUI = false;

    private Camera cam;
    private MatchWidth matchWidth;
    private float targetZoom;
    private Vector3 targetPosition;

    public bool preventPanAndZoom = false;

    Vector3 zoomedOutPos;
    float zoomedOutCamSize;


    void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
    }

    void Start()
    {
        matchWidth = GetComponent<MatchWidth>();
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
        targetPosition = transform.position;


        if (matchWidth != null)
        {
            matchWidth.SetToDesiredWidth();
            minZoom = matchWidth.DesiredWidth - 2f;
            maxZoom = matchWidth.DesiredMaxWidth;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }

    }

    void Update()
    {
        if (preventPanAndZoom)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
        HandleMouseZoom();
#else
    HandleTouchDrag();
    HandleTouchZoom();
#endif

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomLerpSpeed);
    }


    void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startedOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (startedOverUI) return;

            lastPanPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            if (startedOverUI) return;

            Vector2 delta = (Vector2)Input.mousePosition - lastPanPosition;
            PanCamera(delta);
            lastPanPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            startedOverUI = false;
        }
    }

    void HandleTouchDrag()
    {
        var validTouches = Input.touches
            .Where(touch => !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            .ToArray();

        if (Input.touchCount == 1 && currentGesture != GestureMode.Zoom)
        {
            Touch touch = validTouches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    lastPanPosition = touch.position;
                    isDragging = true;
                    currentGesture = GestureMode.Pan;
                    break;

                case TouchPhase.Moved:
                    if (isDragging && currentGesture == GestureMode.Pan)
                    {
                        Vector2 delta = touch.position - lastPanPosition;
                        PanCamera(delta);
                        lastPanPosition = touch.position;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    currentGesture = GestureMode.None;
                    break;
            }
        }
    }


    void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float scaledSpeed = zoomSpeedMouse * (cam.orthographicSize / maxZoom);
            ZoomCamera(-scroll * scaledSpeed);
        }
    }

    void HandleTouchZoom()
    {
        if (Input.touchCount == 2)
        {
            if (currentGesture == GestureMode.None)
                currentGesture = GestureMode.Zoom;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 prevTouch0 = touch0.position - touch0.deltaPosition;
            Vector2 prevTouch1 = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (prevTouch0 - prevTouch1).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            ZoomCamera(-difference * zoomSpeedTouch);
        }
        else if (currentGesture == GestureMode.Zoom)
        {
            // Reset gesture mode when fingers lifted
            currentGesture = GestureMode.None;
        }
    }


    void ZoomCamera(float increment)
    {
        targetZoom = Mathf.Clamp(targetZoom + increment, minZoom, maxZoom);
    }

    void PanCamera(Vector3 delta)
    {
        Vector3 move = cam.ScreenToWorldPoint(cam.transform.position + delta) - cam.ScreenToWorldPoint(cam.transform.position);
        move.z = 0;

        Vector3 newPosition = targetPosition - move * dragSpeed;
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        targetPosition = newPosition;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 center = new Vector3(
            (minBounds.x + maxBounds.x) / 2f,
            (minBounds.y + maxBounds.y) / 2f,
            transform.position.z);

        Vector3 size = new Vector3(
            Mathf.Abs(maxBounds.x - minBounds.x),
            Mathf.Abs(maxBounds.y - minBounds.y),
            0.1f);

        Gizmos.DrawWireCube(center, size);
    }

    public void AdjustBoundary(List<Seat> seats)
    {
        if (seats == null || seats.Count == 0)
            return;

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var seat in seats)
        {
            Bounds bounds = seat.GetBounds();
            Vector3 center = seat.transform.position;
            Vector3 extents = bounds.extents;

            float seatMinX = center.x - extents.x;
            float seatMinY = center.y - extents.y;
            float seatMaxX = center.x + extents.x;
            float seatMaxY = center.y + extents.y;

            minX = Mathf.Min(minX, seatMinX);
            minY = Mathf.Min(minY, seatMinY);
            maxX = Mathf.Max(maxX, seatMaxX);
            maxY = Mathf.Max(maxY, seatMaxY);
        }

        minBounds = new Vector2(minX - boundaryPadding, minY - boundaryPadding);
        maxBounds = new Vector2(maxX + boundaryPadding, maxY + boundaryPadding);

        StartCoroutine(ZoomOutFromWholeLevelViewPoint(seats));
    }


    List<Seat> allSeats;
    IEnumerator ZoomOutFromWholeLevelViewPoint(List<Seat> seats)
    {
        yield return new WaitUntil(() => { return cam != null; });

        if (seats == null || seats.Count == 0)
            yield break;

        preventPanAndZoom = true;
        allSeats = seats;

        Vector3 originalPosition = transform.position;
        float originalZoom = targetZoom;

        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        foreach (var seat in seats)
        {
            Bounds bounds = seat.GetBounds();
            Vector3 center = seat.transform.position;
            Vector3 extents = bounds.extents;

            float seatMinX = center.x - extents.x;
            float seatMinY = center.y - extents.y;
            float seatMaxX = center.x + extents.x;
            float seatMaxY = center.y + extents.y;

            minX = Mathf.Min(minX, seatMinX);
            minY = Mathf.Min(minY, seatMinY);
            maxX = Mathf.Max(maxX, seatMaxX);
            maxY = Mathf.Max(maxY, seatMaxY);
        }


        Vector3 boundsCenter = new Vector3((minX + maxX) / 2f, (minY + maxY) / 2f, originalPosition.z);
        Vector2 boundsSize = new Vector2(maxX - minX, maxY - minY);

        float desiredZoom = Mathf.Max(
            boundsSize.y / cam.aspect,
            boundsSize.x / cam.aspect
        ) * 0.6f;

        desiredZoom = Mathf.Clamp(desiredZoom, minZoom, maxZoom);

        transform.position = boundsCenter;
        cam.orthographicSize = desiredZoom;

        zoomedOutPos = boundsCenter;
        zoomedOutCamSize = desiredZoom;


        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < seats.Count; i++)
        {
            if (seats[i].isOpenSeat)
            {
                seats[i].SetOpenSeat();
                yield return new WaitForSeconds(.2f);
            }
        }


        yield return new WaitForSeconds(1f);

        UIManager.Instance.GamePlayElementsIn();

        float duration = .5f;
        float elapsed = 0f;

        var startPos = transform.position;
        var startZoom = cam.orthographicSize;

        while (elapsed < duration)
        {
            float x = elapsed / duration;
            x = x * x * x * (x * (6.0f * x - 15.0f) + 10.0f);
            transform.position = Vector3.Lerp(startPos, originalPosition, x);
            cam.orthographicSize = Mathf.Lerp(startZoom, originalZoom, x);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        cam.orthographicSize = originalZoom;

        preventPanAndZoom = false;
    }

    public void ZoomOut()
    {
        preventPanAndZoom = true;

        if (allSeats == null || allSeats.Count == 0)
            return;

        // Calculate the overall bounds of all seats to find the topmost seat
        float maxY = float.MinValue;

        // Loop through all seats and find the highest Y position (topmost seat)
        foreach (var seat in allSeats)
        {
            Bounds seatBounds = seat.GetBounds(); // Assuming GetBounds() gives the seat's world space bounds
            float seatMaxY = seatBounds.max.y;  // Get the topmost point of each seat
            maxY = Mathf.Max(maxY, seatMaxY);  // Get the highest point across all seats
        }

        // Make the top margin a percentage of the orthographic size
        // For example, 15% of the screen height
        float topMarginPercentage = 0.25f;
        float topScreenMargin = zoomedOutCamSize * topMarginPercentage;

        // Calculate the new camera position
        // In an orthographic camera, the top edge is at cameraY + orthographicSize
        float newCameraY = maxY - (zoomedOutCamSize - topScreenMargin);

        // Create the new camera position (keeping X and Z from zoomedOutPos)
        Vector3 newPosition = new Vector3(zoomedOutPos.x, newCameraY, zoomedOutPos.z);

        // Apply the new camera position smoothly
        transform.DOMove(newPosition, .8f);
        cam.DOOrthoSize(zoomedOutCamSize, .8f);
    }


    Vector2 VptoWP(float x, float y)
    {
        return Camera.main.ViewportToWorldPoint(new(x, y, 0));

    }

    public IEnumerator MoveToPosition(Vector2 newTargetPos, float duration)
    {
        preventPanAndZoom = true;

        // Clamp target position within boundaries

        Vector3 startTarget = transform.position;
        Vector3 target = new(newTargetPos.x, newTargetPos.y, transform.position.z);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // Smoothstep

            transform.position = Vector3.Lerp(startTarget, target, t);
            targetPosition = transform.position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        preventPanAndZoom = false;
    }



}
