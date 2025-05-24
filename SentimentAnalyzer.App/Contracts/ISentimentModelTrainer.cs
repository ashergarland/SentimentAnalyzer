namespace SentimentAnalyzer.App.Contracts
{
    using System.Threading.Tasks;
    using SentimentAnalyzer.App.Models;

    public interface ISentimentModelTrainer
    {
        /// <summary>
        /// Trains a binary classification model on the provided training data and evaluates on test data.
        /// </summary>
        /// <param name="trainPath">Path to the CSV training dataset</param>
        /// <param name="testPath">Path to the CSV test dataset</param>
        /// <returns>Evaluation metrics summary</returns>
        Task<ModelTrainingResult> TrainAndEvaluateAsync(string trainPath, string testPath);
    }
}
