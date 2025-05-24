using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SentimentAnalyzer.App.Contracts;
using SentimentAnalyzer.App.Preprocessing;
using Xunit;

namespace SentimentAnalyzer.Tests.PreprocessingTests
{
    public class SentimentDataLoaderTests
    {
        private readonly ISentimentDataLoader _loader;

        private static string GetPath(string path)
        {
            return Path.Combine(AppContext.BaseDirectory, "data", path);
        }

        public SentimentDataLoaderTests()
        {
            _loader = new CsvSentimentDataLoader(); // You'll implement this
        }

        [Fact]
        public async Task Loads_Correct_Number_Of_Examples()
        {
            var data = await _loader.LoadAsync(GetPath("labeled_sentiment_data.csv"));
            Assert.Equal(4, data.Count);
        }

        [Fact]
        public async Task Trims_And_Normalizes_Text()
        {
            var data = await _loader.LoadAsync(GetPath("labeled_sentiment_data.csv"));
            var first = data.First();

            Assert.Equal("positive", first.Sentiment);
            Assert.Equal("i absolutely loved this!", first.Text);
        }

        [Fact]
        public async Task Handles_Empty_Lines_Gracefully()
        {
            var data = await _loader.LoadAsync(GetPath("test_with_blank_lines.csv"));
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task Skips_Rows_With_Missing_Fields()
        {
            var data = await _loader.LoadAsync(GetPath("test_missing_fields.csv"));
            Assert.Single(data);
            Assert.Equal("negative", data.First().Sentiment);
        }

        [Fact]
        public async Task Ignores_Extra_Columns()
        {
            var data = await _loader.LoadAsync(GetPath("test_extra_columns.csv"));
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task Allows_Duplicate_Entries()
        {
            var data = await _loader.LoadAsync(GetPath("test_duplicates.csv"));
            Assert.Equal(2, data.Count);
        }

        [Fact]
        public async Task Throws_If_File_Does_Not_Exist()
        {
            var ex = await Assert.ThrowsAsync<FileNotFoundException>(async () =>
                await _loader.LoadAsync("data/file_does_not_exist.csv"));
            Assert.Contains("file_does_not_exist.csv", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Normalizes_Whitespace_In_Text()
        {
            var data = await _loader.LoadAsync(GetPath("test_whitespace.csv"));
            var cleaned = data.First().Text;

            Assert.Equal("this is a test.", cleaned);
        }

        [Fact]
        public async Task Fails_On_Header_Mismatch()
        {
            await Assert.ThrowsAsync<FormatException>(() => _loader.LoadAsync(GetPath("test_header_mismatch.csv")));
        }
    }
}
