using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryPage : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI storyNameText;
    [SerializeField] Transform chapterButtonContainer;

    public void Initialize(StoryModeManager.Story story, GameObject chapterButtonPrefab, int index)
    {
        storyNameText.text = story.storyName;
        // Example implementation: Populate the StoryPage with chapters
        for (int i = 0; i < story.chapters.Count; i++)
        {
            int currentChapter = (index * 3) + i + 1; // Assuming 3 chapters per story page

            GameObject chapterButton = Instantiate(chapterButtonPrefab, chapterButtonContainer);

            if (currentChapter == GameData.CurrentLevel)
            {
                chapterButton.GetComponent<ChapterButton>().Initialize(story.chapters[i], false, false);
            }
            else if (currentChapter < GameData.CurrentLevel)
            {
                chapterButton.GetComponent<ChapterButton>().Initialize(story.chapters[i], true, false);
            }
            else
            {
                chapterButton.GetComponent<ChapterButton>().Initialize(story.chapters[i], false, true);
            }

        }
    }
}