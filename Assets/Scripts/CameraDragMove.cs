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

    public void AdjustBoundary(List<Bounds> bounds)
    {
        if (bounds == null || bounds.Count == 0)
            return;

        Bounds combined = bounds[0];
        for (int i = 1; i < bounds.Count; i++)
        {
            combined.Encapsulate(bounds[i]);
        }

        // Apply padding
        combined.Expand(boundaryPadding * 2); // Padding applies to both sides

        minBounds = new Vector2(combined.min.x, combined.min.y);
        maxBounds = new Vector2(combined.max.x, combined.max.y);
    }
}
