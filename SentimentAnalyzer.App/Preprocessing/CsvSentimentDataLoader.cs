namespace SentimentAnalyzer.App.Preprocessing
{
    using System.Globalization;
    using CsvHelper;
    using CsvHelper.Configuration;
    using SentimentAnalyzer.App.Contracts;
    using SentimentAnalyzer.App.Models;

    public sealed class CsvSentimentDataLoader : ISentimentDataLoader
    {
        public async Task<IReadOnlyCollection<SentimentExample>> LoadAsync(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");

            var results = new List<SentimentExample>();
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = System.Text.Encoding.UTF8
            });
            csv.Context.RegisterClassMap<SentimentExampleMap>();

            try
            {
                await foreach (var record in csv.GetRecordsAsync<SentimentExample>())
                {
                    if (!string.IsNullOrWhiteSpace(record.Sentiment) &&
                        !string.IsNullOrWhiteSpace(record.Text))
                    {
                        results.Add(record);
                    }
                }
            }
            catch (MissingFieldException ex)
            {
                throw new FormatException("Header must have exactly two columns: 'Sentiment' and 'Text'.", ex.InnerException);
            }

            return results;
        }
    }
}