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

    private void Start()
    {
        InitializeStoryPages();
    }

    private void InitializeStoryPages()
    {
        for (int i = 0; i < stories.Count; i++)
        {
            GameObject storyPage = Instantiate(storyPagePrefab, storyPageContainer);
            storyPage.GetComponent<StoryPage>().Initialize(stories[i], chapterButtonPrefab, i);
            pageSlider._pages.Add(storyPage.GetComponent<PageContainer>());
        }
    }

}
