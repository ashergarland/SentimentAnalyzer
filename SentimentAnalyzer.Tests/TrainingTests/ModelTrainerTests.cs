namespace SentimentAnalyzer.Tests.TrainingTests
{
    using System.Threading.Tasks;
    using SentimentAnalyzer.App.Contracts;
    using SentimentAnalyzer.App.Models;
    using SentimentAnalyzer.App.Preprocessing;
    using SentimentAnalyzer.App.Training;
    using SentimentAnalyzer.Tests;
    using Xunit;

    public class ModelTrainerTests
    {
        private readonly ISentimentModelTrainer _trainer;
        private readonly ISentimentDataLoader _loader;

        public ModelTrainerTests()
        {
            _loader = new CsvSentimentDataLoader(); // Stage 1 implementation
            _trainer = new SentimentModelTrainer(); // You'll implement this for stage 2
        }

        [Fact]
        public async Task TrainAndEvaluate_ShouldReturnReasonableMetrics()
        {
            // Arrange
            var trainPath = TestHelper.GetPath("stage2\\train_sentiment_data.csv");
            var testPath = TestHelper.GetPath("stage2\\test_sentiment_data.csv");

            var trainExamples = await _loader.LoadAsync(trainPath);
            var testExamples = await _loader.LoadAsync(testPath);

            // Act
            var result = _trainer.TrainAndEvaluate(trainExamples, testExamples);

            // Assert
            Assert.InRange(result.Accuracy, 0.5, 1.0);
            Assert.InRange(result.Auc, 0.5, 1.0);
            Assert.InRange(result.F1Score, 0.5, 1.0);
        }

        [Fact]
        public void TrainAndEvaluate_ShouldThrowIfTrainIsNull()
        {
            var testData = new List<SentimentExample> { new() { Sentiment = "positive", Text = "Great" } };
            Assert.Throws<ArgumentNullException>(() => _trainer.TrainAndEvaluate(null!, testData));
        }

        [Fact]
        public void TrainAndEvaluate_ShouldThrowIfTrainIsEmpty()
        {
            var train = new List<SentimentExample>();
            var test = new List<SentimentExample> { new() { Sentiment = "positive", Text = "Hello" } };
            Assert.Throws<InvalidOperationException>(() => _trainer.TrainAndEvaluate(train, test));
        }

        [Fact]
        public void TrainAndEvaluate_ShouldThrowIfMissingText()
        {
            var train = new List<SentimentExample>
            {
                new() { Sentiment = "positive", Text = "" }
            };
            var test = new List<SentimentExample>
            {
                new() { Sentiment = "negative", Text = "Awful" }
            };

            Assert.Throws<FormatException>(() => _trainer.TrainAndEvaluate(train, test));
        }

        [Fact]
        public void TrainAndEvaluate_SucceedsWithMinimalData()
        {
            var train = new List<SentimentExample>
            {
                new() { Sentiment = "positive", Text = "Amazing product" },
                new() { Sentiment = "negative", Text = "Horrible service" }
            };

            var test = new List<SentimentExample>
            {
                new() { Sentiment = "positive", Text = "Great" },
                new() { Sentiment = "negative", Text = "Terrible" }
            };

            var result = _trainer.TrainAndEvaluate(train, test);

            Assert.InRange(result.Accuracy, 0.0, 1.0);
        }
    }
}
