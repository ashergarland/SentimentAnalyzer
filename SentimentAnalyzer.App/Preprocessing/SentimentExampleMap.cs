namespace SentimentAnalyzer.App.Preprocessing
{
    using CsvHelper.Configuration;
    using SentimentAnalyzer.App.Models;

    public sealed class SentimentExampleMap : ClassMap<SentimentExample>
    {
        public SentimentExampleMap()
        {
            Map(m => m.Sentiment).Name("Sentiment").Convert(args =>
            {
                var value = args.Row.GetField("Sentiment")?.Trim().ToLower();
                return string.IsNullOrEmpty(value) ? null : value;
            });

            Map(m => m.Text).Name("Text").Convert(args =>
            {
                var value = args.Row.GetField("Text")?.Trim();
                return string.IsNullOrEmpty(value)
                    ? null
                    : TextNormalizer.Normalize(value).ToLower();
            });
        }
    }
}
