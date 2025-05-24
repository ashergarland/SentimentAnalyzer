namespace SentimentAnalyzer.App.Preprocessing
{
    using System.Text.RegularExpressions;
    public static class TextNormalizer
    {
        private static readonly Regex CollapseWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        public static string Normalize(string input)
        {
            var unescaped = Regex.Unescape(input);
            return CollapseWhitespaceRegex.Replace(unescaped, " ").Trim();
        }
    }
}
