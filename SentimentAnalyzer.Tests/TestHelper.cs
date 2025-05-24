namespace SentimentAnalyzer.Tests
{
    public static class TestHelper
    {
        public static string GetPath(string path)
        {
            return Path.Combine(AppContext.BaseDirectory, "data", path);
        }
    } 
}        