using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshPro))]
[RequireComponent(typeof(Collider))] // Needed for Physics Raycast
public class ClickableWordsHandler : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshPro textMeshPro;

    private readonly Dictionary<int, string> wordLinkLookup = new();
    public static event Action<string> OnWordClicked;

    private void Awake()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// Initializes clickable words in the current text based on the list provided.
    /// Each word will be wrapped in a <link> tag with a unique index.
    /// </summary>
    public void InitializeClickableWords(List<string> clickableWords)
    {
        string originalText = textMeshPro.text;
        wordLinkLookup.Clear();

        int linkIndex = 0;

        foreach (var word in clickableWords)
        {
            // Word boundary pattern: replace only full words, not substrings
            string pattern = $@"\b{Regex.Escape(word)}\b";
            if (Regex.IsMatch(originalText, pattern))
            {
                string linkTag = $"<link={linkIndex}>{word}</link>";
                originalText = Regex.Replace(originalText, pattern, linkTag, RegexOptions.IgnoreCase);
                wordLinkLookup[linkIndex] = word;
                linkIndex++;
            }
        }

        textMeshPro.text = originalText;
    }

    /// <summary>
    /// Detects clicks on links and fires an event with the clicked word.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Camera cam = Camera.main;
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, cam);

        if (linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            if (int.TryParse(linkInfo.GetLinkID(), out int id) && wordLinkLookup.TryGetValue(id, out string word))
            {
                Debug.Log($"Clicked word: {word}");
                OnWordClicked?.Invoke(word);
            }
        }
    }
}
