using UnityEngine;


[RequireComponent(typeof(Camera))]
public class MatchWidth : MonoBehaviour
{

    public static MatchWidth Instance { get; private set; }
    // Set this to the in-world distance between the left & right edges of your scene.
    public float sceneWidth = 10;
    float desiredWidth;

    [HideInInspector]
    public float DesiredWidth
    {
        get { return desiredWidth; }
    }

    public float DesiredMaxWidth
    {
        get { return desiredWidth + 4f; }
    }

    Camera _camera;
    void Awake()
    {
        Instance = this;
        _camera = GetComponent<Camera>();
        desiredWidth = GetDesiredWidth();
        SetToDesiredWidth();
    }

    float GetDesiredWidth()
    {
        float unitsPerPixel = sceneWidth / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        return desiredHalfHeight;
    }

    public void SetToDesiredWidth()
    {

        _camera.orthographicSize = desiredWidth;
    }

    public float GetCurrentSizeRatio()
    {
        return DesiredWidth / _camera.orthographicSize;
    }


}
