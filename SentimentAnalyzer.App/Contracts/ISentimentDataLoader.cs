namespace SentimentAnalyzer.App.Contracts
{
    using SentimentAnalyzer.App.Models;
    /// <summary>
    /// Loads labeled sentiment examples from a CSV file.
    /// </summary>
    public interface ISentimentDataLoader
    {
        /// <summary>
        /// Loads sentiment-labeled text data from a CSV file, normalizing and validating each record.
        /// </summary>
        /// <param name="filePath">The path to a UTF-8 encoded CSV file with a header containing at least "Sentiment" and "Text" columns (case-insensitive).</param>
        /// <returns>
        /// A read-only collection of <see cref="SentimentExample"/> instances representing normalized records.
        /// </returns>
        /// <remarks>
        /// Requirements:
        /// - The CSV file must include a header containing at least the columns: Sentiment and Text (case-insensitive).
        /// - Extra columns beyond these two are allowed and must be ignored.
        /// - Each row must contain non-empty values for both the Sentiment and Text columns to be included.
        /// - Empty lines or malformed rows (e.g., missing required fields) must be ignored.
        /// - The "Text" field must be normalized:
        ///   - Trim leading/trailing whitespace
        ///   - Convert to lowercase
        ///   - Collapse multiple spaces/tabs/newlines into a single space
        /// - The "Sentiment" field must be normalized:
        ///   - Trim and convert to lowercase
        /// - Duplicate lines are allowed.
        /// - If the file does not exist or cannot be read, an appropriate exception must be thrown (e.g., <see cref="System.IO.FileNotFoundException"/>).
        /// </remarks>
        Task<IReadOnlyCollection<SentimentExample>> LoadAsync(string filePath);
    }
}