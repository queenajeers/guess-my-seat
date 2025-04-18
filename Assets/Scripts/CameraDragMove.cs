using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraDragMove : MonoBehaviour
{
    public static CameraDragMove Instance { get; private set; }

    [Header("Drag Settings")]
    public float dragSpeed = 1f;

    [Header("Movement Boundaries")]
    public Vector2 minBounds = new Vector2(-10, -10);
    public Vector2 maxBounds = new Vector2(10, 10);

    private Vector2 lastPanPosition;
    private bool isDragging = false;
    private bool startedOverUI = false;

    private Camera cam;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseDrag();
#else
        HandleTouchDrag();
#endif
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

        if (validTouches.Length == 1)
        {
            Touch touch = validTouches[0];

            if (touch.phase == TouchPhase.Began)
            {
                lastPanPosition = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector3 delta = touch.position - lastPanPosition;
                PanCamera(delta);
                lastPanPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
    }

    void PanCamera(Vector3 delta)
    {
        Vector3 move = cam.ScreenToWorldPoint(cam.transform.position + delta) - cam.ScreenToWorldPoint(cam.transform.position);
        move.z = 0;

        Vector3 newPosition = transform.position - move * dragSpeed;
        newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

        transform.position = newPosition;
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

    public float boundaryPadding = 1f; // Optional extra space around all bounds

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
            // Assuming seat.transform.position gives the center of the seat
            // and seat.GetBounds() returns a Bounds object with extents
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

        // Apply padding
        minBounds = new Vector2(minX - boundaryPadding, minY - boundaryPadding);
        maxBounds = new Vector2(maxX + boundaryPadding, maxY + boundaryPadding);
    }

}
