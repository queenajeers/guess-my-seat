using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public static class TextStyler
{
    /// <summary>
    /// Converts a normal string into a string with underline tags over specified words.
    /// Use with TextMeshPro text components.
    /// </summary>
    /// <param name="normalString">The base text.</param>
    /// <param name="highlightWords">List of words to be underlined.</param>
    /// <returns>Formatted string with underlines on specified words.</returns>
    public static string GiveHyperText(string normalString, List<string> highlightWords)
    {
        if (string.IsNullOrEmpty(normalString) || highlightWords == null || highlightWords.Count == 0)
            return normalString;

        string pattern = @"\b(" + string.Join("|", highlightWords) + @")\b";

        string result = Regex.Replace(normalString, pattern, match => $"<u>{match.Value}</u>", RegexOptions.IgnoreCase);
        return result;
    }
}
