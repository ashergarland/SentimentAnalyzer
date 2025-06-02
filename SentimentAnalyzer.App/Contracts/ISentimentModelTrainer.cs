namespace SentimentAnalyzer.App.Contracts
{
    using SentimentAnalyzer.App.Models;
    /// <summary>
    /// Trains a binary sentiment classifier and evaluates its performance using in-memory normalized data.
    /// </summary>
    public interface ISentimentModelTrainer
    {
        /// <summary>
        /// Trains and evaluates a binary classifier using labeled, preprocessed sentiment examples.
        /// </summary>
        /// <param name="trainExamples">The training dataset (must be normalized)</param>
        /// <param name="testExamples">The evaluation dataset (must be normalized)</param>
        /// <returns>
        /// A <see cref="ModelTrainingResult"/> containing Accuracy, AUC, and F1 Score.
        /// </returns>
        /// <remarks>
        /// Input Requirements:
        /// - Both collections must be non-null and contain at least one example.
        /// - Each example must have non-empty, normalized "Sentiment" and "Text" fields.
        /// - Sentiment labels must represent binary classification (e.g., positive/negative).
        ///
        /// Behavior:
        /// - The examples are converted to ML.NET IDataViews internally.
        /// - The "Sentiment" field is mapped to a key type using MapValueToKey (as Label).
        /// - The "Text" field is featurized using FeaturizeText (as Features).
        /// - A binary classifier is trained using SdcaLogisticRegression.
        /// - The trained model is evaluated using BinaryClassificationCatalog.Evaluate().
        ///
        /// Edge Cases:
        /// - If either collection is null, throw <see cref="ArgumentNullException"/>.
        /// - If either contains fewer than one example, throw <see cref="InvalidOperationException"/>.
        /// </remarks>
        ModelTrainingResult TrainAndEvaluate(
            IEnumerable<SentimentExample> trainExamples,
            IEnumerable<SentimentExample> testExamples);
    }
}
