# Sentiment Analyzer

## 🛠️ ENVIRONMENT SETUP — Sentiment Analyzer in C\#

This guide sets up your workspace for building a multi-stage ML.NET project with clean architecture and test-driven development.

---

### ✅ STEP 1 — Scaffold the Solution

Run these in your terminal:

```bash
# Create a new solution file
dotnet new sln -n SentimentAnalyzer

# Create the main application project
dotnet new console -n SentimentAnalyzer.App

# Create the test project using xUnit
dotnet new xunit -n SentimentAnalyzer.Tests

# Add both projects to the solution
dotnet sln add SentimentAnalyzer.App
dotnet sln add SentimentAnalyzer.Tests

# Reference the app project from the test project
dotnet add SentimentAnalyzer.Tests reference SentimentAnalyzer.App
```

Your directory structure will now look like this:

```
SentimentAnalyzer.sln
├── SentimentAnalyzer.App/
└── SentimentAnalyzer.Tests/
```

---

### ✅ STEP 2 — Install Required NuGet Packages

#### In `SentimentAnalyzer.App`:

```bash
cd SentimentAnalyzer.App

dotnet add package Microsoft.ML --version 4.0.2
dotnet add package Microsoft.ML.DataView --version 4.0.2
dotnet add package Microsoft.Extensions.Logging.Console
```

#### In `SentimentAnalyzer.Tests`:

```bash
cd ../SentimentAnalyzer.Tests

dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

---

### ✅ STEP 3 — Recommended Project Structure

Once created, organize like this:

```
SentimentAnalyzer.sln
├── SentimentAnalyzer.App/
│   ├── Contracts/              # Interfaces
│   ├── Preprocessing/          # Stage 1 logic
│   └── ...
├── SentimentAnalyzer.Tests/
│   ├── PreprocessingTests/     # Stage 1 tests
│   └── ...
└── data/
    └── labeled_sentiment_data.csv  # Input training/test data
```

---

✅ You're now fully set up and ready for **Stage 1 – Data Loading + Preprocessing**. Let me know when to start and I’ll provide:

* Interface definitions
* Sample CSV format
* xUnit test cases
* ML.NET API explanations
