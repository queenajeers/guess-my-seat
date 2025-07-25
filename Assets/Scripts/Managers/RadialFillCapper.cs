using UnityEngine;
using UnityEngine.UI;

public class RadialFillCapper : MonoBehaviour
{
    [Header("References")]
    public Image radialFillImage; // The main radial filled image
    public RectTransform startCap; // Circle image for start of the fill
    public RectTransform endCap;   // Circle image for end of the fill
    public RectTransform endCapArrow; // Arrow image for end cap


    [Header("Settings")]
    public float radius = 100f; // Distance from center to cap


    private void Start()
    {
        if (radialFillImage != null && startCap != null && endCap != null)
        {
            // Set the cap images' colors to match the radial fill image's color
            Image startCapImage = startCap.GetComponent<Image>();
            Image endCapImage = endCap.GetComponent<Image>();

            if (startCapImage != null)
                startCapImage.color = radialFillImage.color;

            if (endCapImage != null)
                endCapImage.color = radialFillImage.color;
        }

        if (radialFillImage != null && endCapArrow != null)
        {
            // Set a lighter version of the fill color to the end cap arrow Image
            Image endCapArrowImage = endCapArrow.GetComponent<Image>();

            if (endCapArrowImage != null)
            {
                Color fillColor = radialFillImage.color;
                Color lighterColor = fillColor * 1.2f; // Increase brightness
                lighterColor.a = fillColor.a; // Preserve original alpha
                endCapArrowImage.color = lighterColor;
            }
        }
    }

    void Update()
    {
        if (radialFillImage == null || startCap == null || endCap == null)
            return;

        float fill = radialFillImage.fillAmount;

        // Clamp fill to avoid rotation issues at 0 or full
        fill = Mathf.Clamp01(fill);

        // Radial fill goes clockwise from 0 (up), so angle is 0 - 360 * fill
        float startAngle = 0f;
        float endAngle = 360f * fill;

        // Convert angles to radians
        float startRad = Mathf.Deg2Rad * startAngle;
        float endRad = Mathf.Deg2Rad * endAngle;

        // Set positions
        Vector2 center = Vector2.zero;
        Vector2 startPos = center + new Vector2(Mathf.Sin(startRad), Mathf.Cos(startRad)) * radius;
        Vector2 endPos = center + new Vector2(Mathf.Sin(endRad), Mathf.Cos(endRad)) * radius;

        startCap.anchoredPosition = startPos;
        endCap.anchoredPosition = endPos;

        // Optional: Rotate the cap to align it if needed
        startCap.localRotation = Quaternion.Euler(0, 0, -startAngle);
        endCap.localRotation = Quaternion.Euler(0, 0, -endAngle);

        // Rotate the end cap arrow to align with the tangent
        if (endCapArrow != null)
        {
            fill = Mathf.Clamp01(radialFillImage.fillAmount);
            endAngle = 360f * fill;
            endRad = Mathf.Deg2Rad * endAngle;

            // Tangent vector (clockwise direction)
            Vector2 tangent = new Vector2(Mathf.Cos(endRad), -Mathf.Sin(endRad));

            endCapArrow.transform.right = tangent;

        }
    }
}
