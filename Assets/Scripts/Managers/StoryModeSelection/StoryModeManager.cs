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
            targetChapterButton = storyPage.GetComponent<StoryPage>().Initialize(stories[i], chapterButtonPrefab, i);
            pageSlider._pages.Add(storyPage.GetComponent<PageContainer>());
        }

        if (targetChapterButton != null)
        {
            fingerUI.transform.SetParent(targetChapterButton.center);
            fingerUI.anchoredPosition = new Vector2(70, -60);
        }

    }



}
