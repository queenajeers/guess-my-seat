using System.Collections.Generic;
using UnityEngine;

public class PeopleScrollManager : MonoBehaviour
{
    public static PeopleScrollManager Instance { get; private set; }
    public List<PersonItem> personItems;
    public RectTransform peopleContent;
    public RectTransform scrollViewRect;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Invoke(nameof(ScrollToLeftEnd), .4f);
    }

    void OnEnable()
    {
        ClickableWordsHandler.OnWordClicked += HandleWordClicked;
    }

    void OnDisable()
    {
        ClickableWordsHandler.OnWordClicked -= HandleWordClicked;
    }

    void HandleWordClicked(string word)
    {
        // You can do per-word logic here
        var targetPerson = personItems.Find(x => x.PersonName.Contains(word));
        if (targetPerson != null)
        {
            Debug.Log("Scrolling to word: " + word);
            ScrollToPerson(targetPerson);
            targetPerson.MakeIconJump();
        }
    }

    void ScrollToLeftEnd()
    {
        float halfContentWidth = peopleContent.rect.width * 0.5f;
        float halfViewportWidth = scrollViewRect.rect.width * 0.5f;

        // Shift content so its left edge aligns with left of viewport
        Vector2 anchoredPos = peopleContent.anchoredPosition;
        anchoredPos.x = halfContentWidth - halfViewportWidth;
        peopleContent.anchoredPosition = anchoredPos;
    }

    void ScrollToRightEnd()
    {
        float halfContentWidth = peopleContent.rect.width * 0.5f;
        float halfViewportWidth = scrollViewRect.rect.width * 0.5f;

        // Shift content so its right edge aligns with right of viewport
        Vector2 anchoredPos = peopleContent.anchoredPosition;
        anchoredPos.x = -(halfContentWidth - halfViewportWidth);
        peopleContent.anchoredPosition = anchoredPos;
    }

    void ScrollToPerson(PersonItem item)
    {
        RectTransform itemRect = item.GetComponent<RectTransform>();

        // Convert item position to world space and then to local point in scrollViewRect
        Vector3[] itemWorldCorners = new Vector3[4];
        itemRect.GetWorldCorners(itemWorldCorners);

        Vector3[] viewWorldCorners = new Vector3[4];
        scrollViewRect.GetWorldCorners(viewWorldCorners);

        // Check if the item is fully within the visible scroll view
        bool isCompletelyVisible =
            itemWorldCorners[0].x >= viewWorldCorners[0].x && // left
            itemWorldCorners[2].x <= viewWorldCorners[2].x;   // right

        if (!isCompletelyVisible)
        {
            float itemLocalX = itemRect.localPosition.x;
            ScrollToElement(itemLocalX);
        }
    }


    void ScrollToElement(float xPosInsidePeopleContent)
    {
        float halfContentWidth = peopleContent.rect.width * 0.5f;
        float halfViewportWidth = scrollViewRect.rect.width * 0.5f;

        // Convert local X (relative to center-pivoted content) into offset
        float offsetFromCenter = xPosInsidePeopleContent - 0f; // since pivot is center, 0 = center

        Vector2 anchoredPos = peopleContent.anchoredPosition;
        anchoredPos.x = -offsetFromCenter; // negative to move content in the correct direction

        // Clamp so you can't scroll beyond edges
        float maxOffset = halfContentWidth - halfViewportWidth;
        anchoredPos.x = Mathf.Clamp(anchoredPos.x, -maxOffset, maxOffset);

        peopleContent.anchoredPosition = anchoredPos;
    }

}
