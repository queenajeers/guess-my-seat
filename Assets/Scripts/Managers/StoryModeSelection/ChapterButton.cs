
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

    public Transform center;

    bool isSelected = false;
    int linkedLevel = 0;

    public void Initialize(StoryModeManager.Chapter chapter, bool isFinished, bool isLocked, int level)
    {
        chapterNameText.text = chapter.chapterName;
        posterImage.sprite = chapter.chapterIcon;

        finishedUI.SetActive(isFinished);
        lockedUI.SetActive(isLocked);
        playUI.SetActive(!isFinished && !isLocked);
        isSelected = !isFinished && !isLocked;
        linkedLevel = level;
    }
    public void OnPlayButtonClicked()
    {
        if (isSelected)
        {
            // Handle play button click
            Debug.Log($"Playing chapter: {chapterNameText.text}, Level: {linkedLevel}");
            // Add your logic to start the chapter here
        }
    }



}
