
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterButton : MonoBehaviour
{
    public Image posterImage;
    public TextMeshProUGUI chapterNameText;
    public GameObject finishedUI;
    public GameObject lockedUI;
    public GameObject playUI;

    public void Initialize(StoryModeManager.Chapter chapter, bool isFinished, bool isLocked)
    {
        chapterNameText.text = chapter.chapterName;
        posterImage.sprite = chapter.chapterIcon;

        finishedUI.SetActive(isFinished);
        lockedUI.SetActive(isLocked);
        playUI.SetActive(!isFinished && !isLocked);
    }
    public void OnPlayButtonClicked()
    {
        // Handle play button click
        Debug.Log($"Playing chapter: {chapterNameText.text}");
        // Add your logic to start the chapter here
    }

}
