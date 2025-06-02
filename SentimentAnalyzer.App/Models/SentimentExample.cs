namespace SentimentAnalyzer.App.Models
{
    public class SentimentExample
    {
        public string Sentiment { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsPositive => Sentiment?.Equals("positive", StringComparison.OrdinalIgnoreCase) ?? false;
    }
}
