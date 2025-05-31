using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public RectTransform loadingBar;
    public float minWidth = 0f;
    public float maxWidth = 200f;
    public float animationDuration = 2f; // Duration of the animation in seconds

    public TextMeshProUGUI loadingText;

    // public int FirstLevel;

    void Awake()
    {
        Application.targetFrameRate = 60;
        // GameData.CurrentLevel = FirstLevel;
    }
    private void Start()
    {
        // Start the loading bar animation and text animation when the script starts
        StartLoadingBarAnimation();
        StartLoadingTextAnimation();
    }

    IEnumerator LoadingBarAnimation()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / animationDuration);
            float currentWidth = Mathf.Lerp(minWidth, maxWidth, progress);

            // Update the width of the loading bar
            if (loadingBar != null)
            {
                loadingBar.sizeDelta = new Vector2(currentWidth, loadingBar.sizeDelta.y);
            }

            yield return null; // Wait for the next frame
        }

        // Ensure the loading bar reaches the max width at the end
        if (loadingBar != null)
        {
            loadingBar.sizeDelta = new Vector2(maxWidth, loadingBar.sizeDelta.y);
        }

        if (GameData.CurrentLevel == 0)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    IEnumerator LoadingTextAnimation()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true) // Infinite loop to keep animating the text
        {
            dotCount = (dotCount + 1) % 6; // Cycle through 0, 1, 2, 3
            loadingText.text = baseText + new string('.', dotCount); // Append dots to the base text
            yield return new WaitForSeconds(0.2f); // Wait for 0.5 seconds before updating
        }
    }

    // Example method to start the loading bar animation
    public void StartLoadingBarAnimation()
    {
        StartCoroutine(LoadingBarAnimation());
    }

    // Example method to start the loading text animation
    public void StartLoadingTextAnimation()
    {
        if (loadingText != null)
        {
            StartCoroutine(LoadingTextAnimation());
        }
    }
}
