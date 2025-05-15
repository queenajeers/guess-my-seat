using System.Collections.Generic;
using TS.PageSlider;
using UnityEngine;

public class StoryModeManager : MonoBehaviour
{
    [System.Serializable]
    public class Story
    {
        public string storyName;
        public List<Chapter> chapters;
    }

    [System.Serializable]
    public class Chapter
    {
        public string chapterName;
        public Sprite chapterIcon;
    }

    public List<Story> stories;

    [Header("UI Prefabs")]
    public GameObject storyPagePrefab;
    public GameObject chapterButtonPrefab;

    public Transform storyPageContainer;

    public PageSlider pageSlider;

    public RectTransform fingerUI;

    private void Awake()
    {
        InitializeStoryPages();
    }

    private void InitializeStoryPages()
    {
        int currentLevel = GameData.CurrentLevel;
        pageSlider._startPageIndex = (currentLevel - 1) / 3; // Assuming 3 chapters per story page
        ChapterButton targetChapterButton = null;

        for (int i = 0; i < stories.Count; i++)
        {
            GameObject storyPage = Instantiate(storyPagePrefab, storyPageContainer);
            var chapterButton = storyPage.GetComponent<StoryPage>().Initialize(stories[i], chapterButtonPrefab, i);
            if (chapterButton != null)
            {
                targetChapterButton = chapterButton;
            }
            pageSlider._pages.Add(storyPage.GetComponent<PageContainer>());
        }

        if (targetChapterButton != null)
        {
            Debug.Log($"Target Chapter Button: {targetChapterButton.chapterNameText.text}");
            fingerUI.transform.SetParent(targetChapterButton.center);
            fingerUI.anchoredPosition = new Vector2(70, -60);

            int siblingIndex = targetChapterButton.transform.GetSiblingIndex();

            // Get Previous child if any
            if (siblingIndex > 0)
            {
                targetChapterButton.transform.parent.GetChild(siblingIndex - 1).GetComponent<ChapterButton>().AnimateTick();
            }


        }
    }



}
