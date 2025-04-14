using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class DynamicWordLinker : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI textMeshPro;
    public List<string> targetWords;

    [TextArea] public string baseText;

    [Header("Link Styling")]
    public Color linkColor = Color.cyan; // Editable in Inspector

    public delegate void WordClickedDelegate(string word);
    public static event WordClickedDelegate OnWordClicked;

    void Start()
    {
        if (textMeshPro == null)
            textMeshPro = GetComponent<TextMeshProUGUI>();

        textMeshPro.text = GenerateLinkedText(baseText, targetWords);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshPro, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        {
            var linkInfo = textMeshPro.textInfo.linkInfo[linkIndex];
            string wordID = linkInfo.GetLinkID();
            OnWordClicked?.Invoke(wordID);
        }
    }

    string GenerateLinkedText(string input, List<string> words)
    {
        string hexColor = ColorUtility.ToHtmlStringRGB(linkColor);

        foreach (string word in words)
        {
            string pattern = $@"\b{Regex.Escape(word)}\b";
            string replacement = $"<link=\"{word}\"><color=#{hexColor}><u>{word}</u></color></link>";
            input = Regex.Replace(input, pattern, replacement);
        }

        return input;
    }

}
