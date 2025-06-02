namespace SentimentAnalyzer.App.Training
{
    using SentimentAnalyzer.App.Contracts;
    using SentimentAnalyzer.App.Models;
    using Microsoft.ML;
    public class SentimentModelTrainer : ISentimentModelTrainer
    {
        private readonly MLContext context; // Use a single MLContext instance
        private static readonly int seed = 42; // Set a seed for reproducible training behavior

        public SentimentModelTrainer(MLContext? context = null)
        {
            this.context = context ?? new MLContext(seed: seed);
        }

        public ModelTrainingResult TrainAndEvaluate(
            IEnumerable<SentimentExample> trainExamples,
            IEnumerable<SentimentExample> testExamples)
        {
            ArgumentNullException.ThrowIfNull(trainExamples);
            ArgumentNullException.ThrowIfNull(testExamples);

            if (!trainExamples.Any())
            {
                throw new InvalidOperationException($"Collection {nameof(trainExamples)} must contain at least one example.");
            }

            if (!testExamples.Any())
            {
                throw new InvalidOperationException($"Collection {nameof(testExamples)} must contain at least one example.");
            }

            ValidateExamples(trainExamples, nameof(trainExamples));
            ValidateExamples(testExamples, nameof(testExamples));

            // Step 2. Load input data for model training - Convert to IDataView. 
            IDataView trainData = context.Data.LoadFromEnumerable(SentimentTrainingRow.ConvertFrom(trainExamples));
            IDataView testData = context.Data.LoadFromEnumerable(SentimentTrainingRow.ConvertFrom(testExamples));

            // Step 3. Build data processing and training pipeline
            var pipeline = context.Transforms.Text.FeaturizeText("Features", nameof(SentimentTrainingRow.Text)) // FeaturizeText applies text vectorization
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features")); // SdcaLogisticRegression is ideal for sparse textual features

            // Step 4. Train the model - Efficiently builds the model using your preprocessed dataset
            var model = pipeline.Fit(trainData);

            // Step 5. Evaluate on Test Data - Evaluates model performance using standard metrics
            var predictions = model.Transform(testData);
            var metrics = context.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

            // Step 6. Return Results
            return new ModelTrainingResult
            {
                Accuracy = metrics.Accuracy,
                Auc = metrics.AreaUnderRocCurve,
                F1Score = metrics.F1Score
            };
        }

        private static void ValidateExamples(IEnumerable<SentimentExample> examples, string label)
        {
            foreach (var ex in examples)
            {
                if (string.IsNullOrWhiteSpace(ex.Text) || string.IsNullOrWhiteSpace(ex.Sentiment))
                {
                    throw new FormatException($"Invalid input in {label}: Missing Sentiment or Text.");
                }
            }
        }
    }

    internal class SentimentTrainingRow
    {
        public bool Label { get; set; }
        public string Text { get; set; } = string.Empty;

        public static SentimentTrainingRow ConvertFrom(SentimentExample example) => new SentimentTrainingRow
        {
            Label = example.IsPositive,
            Text = example.Text
        };

        public static IEnumerable<SentimentTrainingRow> ConvertFrom(IEnumerable<SentimentExample> examples) => examples.Select(ConvertFrom);
    }
}
