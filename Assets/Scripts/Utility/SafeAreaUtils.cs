using UnityEngine;

/// <summary>
/// Returns the vertical offset (in world space units) from the top of the screen to the top of the safe area.
/// </summary>
/// <param name="camera">Camera to use for conversion.</param>
/// <param name="safeArea">Safe area Rect in screen space (you can pass Screen.safeArea or a simulated one).</param>
/// <returns>Vertical offset in world space units.</returns>
public static class SafeAreaUtils
{
    public static float GetSafeAreaTopOffsetWorld(Camera camera, Rect safeArea)
    {
        if (camera == null)
        {
            Debug.LogError("Camera is null in GetSafeAreaTopOffsetWorld.");
            return 0f;
        }

        // Top Y position of safe area in screen pixels
        float safeAreaTopY = safeArea.yMax;

        // Convert top of screen and top of safe area to world space
        Vector3 screenTop = new Vector3(Screen.width / 2f, Screen.height, camera.nearClipPlane);
        Vector3 safeAreaTop = new Vector3(Screen.width / 2f, safeAreaTopY, camera.nearClipPlane);

        Vector3 worldTop = camera.ScreenToWorldPoint(screenTop);
        Vector3 worldSafeTop = camera.ScreenToWorldPoint(safeAreaTop);

        return worldTop.y - worldSafeTop.y;
    }
}
