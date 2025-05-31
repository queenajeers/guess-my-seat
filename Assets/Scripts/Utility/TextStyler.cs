using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class TextStyler
{
    /// <summary>
    /// Converts a normal string into a string with underline and bold tags over specified words.
    /// Use with TextMeshPro text components.
    /// </summary>
    /// <param name="normalString">The base text.</param>
    /// <param name="highlightWords">List of words to be underlined and bolded.</param>
    /// <returns>Formatted string with underlines and bold on specified words.</returns>
    public static string GiveHyperText(string normalString, List<string> highlightWords)
    {
        if (string.IsNullOrEmpty(normalString) || highlightWords == null || highlightWords.Count == 0)
            return normalString;

        string pattern = @"\b(" + string.Join("|", highlightWords) + @")\b";

        string result = Regex.Replace(normalString, pattern, match => $"<b><u>{match.Value}</u></b>", RegexOptions.IgnoreCase);
        return result;
    }
}
