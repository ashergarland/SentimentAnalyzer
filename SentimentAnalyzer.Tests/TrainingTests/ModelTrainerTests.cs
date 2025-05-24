namespace SentimentAnalyzer.Tests.TrainingTests
{
    using System.Threading.Tasks;
    using SentimentAnalyzer.App.Contracts;
    using SentimentAnalyzer.Tests;
    using Xunit;
    public class ModelTrainerTests
    {
        private readonly ISentimentModelTrainer _trainer;

        public ModelTrainerTests()
        {
            _trainer = new SentimentModelTrainer(); // You'll implement this
        }

        [Fact]
        public async Task TrainAndEvaluate_ShouldReturnReasonableAccuracy()
        {
            var result = await _trainer.TrainAndEvaluateAsync(
                TestHelper.GetPath("train_sentiment_data.csv"),
                TestHelper.GetPath("test_sentiment_data.csv")
            );

            Assert.InRange(result.Accuracy, 0.5, 1.0); // Avoid random noise
            Assert.InRange(result.Auc, 0.5, 1.0);
            Assert.InRange(result.F1Score, 0.5, 1.0);
        }
    }
}
