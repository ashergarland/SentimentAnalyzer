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

        public async Task<IReadOnlyCollection<SentimentExample>> LoadAsync_manual(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"File not found: {filePath}");

            var sentiments = new List<SentimentExample>();

            using (var reader = new StreamReader(filePath))
            {
                var headerLine = await reader.ReadLineAsync() ?? throw new InvalidDataException("File is empty.");
                var headers = headerLine.Split(',');

                // Validate header
                if (headers.Length >= 2 ||
                    headers[0].Trim() != "Sentiment" ||
                    headers[1].Trim() != "Text")
                {
                    throw new FormatException("Header must have exactly two columns: 'Sentiment' and 'Text'.");
                }

                // Read rows
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue; // empty line -> ignore

                    var commaIndex = line.IndexOf(',');
                    if (commaIndex == -1) continue; // malformed line -> ignore

                    var sentimentRaw = line[..commaIndex].Trim().ToLower();
                    var textRaw = line[(commaIndex + 1)..].Trim();

                    if (textRaw.StartsWith("\"") && textRaw.EndsWith("\""))
                    {
                        textRaw = textRaw[1..^1];
                    }

                    var text = TextNormalizer.Normalize(textRaw).ToLower();

                    if (string.IsNullOrEmpty(sentimentRaw) || string.IsNullOrEmpty(text)) continue; // malformed line -> ignore

                    sentiments.Add(new SentimentExample()
                    {
                        Sentiment = sentimentRaw,
                        Text = text
                    });
                }
            }

            return sentiments;
        }
    }
}