namespace SentimentAnalyzer.App.Models
{
    public class ModelTrainingResult
    {
        public double Accuracy { get; set; }
        public double Auc { get; set; }
        public double F1Score { get; set; }
    }
}
