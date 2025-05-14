using System.Collections.Generic;
using UnityEngine;

public class StoryModeManager : MonoBehaviour
{
    public class Story
    {
        public string storyName;
        public List<Chapter> chapters;
    }
    public class Chapter
    {
        public string chapterName;
        public Sprite chapterIcon;
    }

    public List<Story> stories;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
