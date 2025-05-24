# Sentiment Analyzer

# 🧠 Stage 0 — Environment Setup

## 🛠️ ENVIRONMENT SETUP — Sentiment Analyzer in C\#

This setup prepares your solution to build a **test-driven, multi-stage ML.NET pipeline in C#** using clean architecture, reliable data tooling, and clearly structured test data.

We'll use [CsvHelper](https://joshclose.github.io/CsvHelper/) for robust CSV parsing, enabling us to focus on machine learning logic instead of manual data handling.

---

## ✅ STEP 1 — Scaffold the Solution

Run the following in your terminal:

```bash
# Create the solution and projects
dotnet new sln -n SentimentAnalyzer
dotnet new console -n SentimentAnalyzer.App
dotnet new xunit -n SentimentAnalyzer.Tests

# Add both projects to the solution
dotnet sln add SentimentAnalyzer.App
dotnet sln add SentimentAnalyzer.Tests

# Reference the main app from the test project
dotnet add SentimentAnalyzer.Tests reference SentimentAnalyzer.App
```

---

## ✅ STEP 2 — Install Required NuGet Packages

### In `SentimentAnalyzer.App`:

```bash
cd SentimentAnalyzer.App

dotnet add package Microsoft.ML --version 4.0.2
dotnet add package Microsoft.ML.DataView --version 4.0.2
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package CsvHelper --version 33.0.1
```

### In `SentimentAnalyzer.Tests`:

```bash
cd ../SentimentAnalyzer.Tests

dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Microsoft.NET.Test.Sdk
```

---

## ✅ STEP 3 — Create Folder Structure

This structure supports all five stages of ML development: loading, training, inference, persistence, and deployment.

```
SentimentAnalyzer.sln
├── data/
│   ├── stage1/                  # Preprocessing + validation datasets
│   ├── stage2/                  # Training and evaluation datasets
│   ├── stage3/                  # Inference inputs and expected outputs
│   ├── stage4/                  # Model persistence test files
│   └── stage5/                  # CLI/API sample inputs and test cases
│
├── SentimentAnalyzer.App/
│   ├── Contracts/               # Public interfaces (IDataLoader, ITrainer, etc.)
│   ├── Models/                  # DTOs and result containers
│   ├── Preprocessing/           # Stage 1: text normalization, CSV parsing
│   ├── Training/                # Stage 2: ML pipeline and evaluation
│   ├── Inference/               # Stage 3: prediction logic
│   ├── Persistence/             # Stage 4: model saving/loading
│   ├── Cli/                     # Stage 5: CLI interface
│   └── Api/                     # Stage 5: ASP.NET Web API interface
│
└── SentimentAnalyzer.Tests/
    ├── PreprocessingTests/     # Stage 1 tests
    ├── TrainingTests/          # Stage 2 tests
    ├── InferenceTests/         # Stage 3 tests
    ├── PersistenceTests/       # Stage 4 tests
    ├── ApiTests/               # API integration tests
    ├── CliTests/               # CLI tests
    └── TestUtils/              # Shared test helpers
```

---

## ✅ STEP 4 — Enable Test Data Copying (Required)

To ensure test data is available at runtime, add this to your `SentimentAnalyzer.Tests.csproj`:

```xml
<ItemGroup>
  <None Include="..\data\**\*.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    <Link>data\%(RecursiveDir)%(Filename)%(Extension)</Link>
  </None>
</ItemGroup>
```

This copies all test CSV files into the output `bin/` directory, preserving stage folder structure.

---

## ✅ STEP 5 — Add TestHelper for Path Resolution

File: `SentimentAnalyzer.Tests/TestHelper.cs`

```csharp
using System;
using System.IO;

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
```

Example usage in tests:

```csharp
var path = TestHelper.GetPath("stage2/train_sentiment_data.csv");
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

## 🧠 Stage 1 — Data Loading + Preprocessing

### 🎯 Goal

Build a reusable, test-driven component to:

1. **Load a CSV file** of labeled sentiment data
2. **Return strongly-typed records (`SentimentExample`)**
3. **Normalize and clean text** (trim, lowercase, collapse whitespace)

---

### 📄 Input Dataset Location

| File | Purpose |
| ---- | ------- |
| `data/stage1/labeled_sentiment_data.csv` | Full labeled dataset used in this stage |

#### Sample CSV Format:

```csv
Sentiment,Text
Positive,"I absolutely loved this!"
Negative,"This was terrible and boring"
Positive,"Great experience overall"
Negative,"Not worth the time."
```

---

## 🛠 CSV Parsing Strategy

We use [CsvHelper](https://joshclose.github.io/CsvHelper/) to handle structured CSV input with robust mapping and edge case handling. This simplifies logic and enables strong schema enforcement.

### ✅ Benefits of CsvHelper:

* Automatically maps fields to `SentimentExample`
* Ignores extra/unmapped columns
* Gracefully skips rows with missing data
* Honors quotes, newlines, and whitespace

### 🔧 CsvConfiguration Example

```csharp
var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    TrimOptions = TrimOptions.Trim,
    IgnoreBlankLines = true,
    MissingFieldFound = null // Skip rows missing Sentiment or Text
};
```

---

### 🧼 Normalization Rules

Each row must be normalized before being returned:

* ✅ Trim leading/trailing whitespace
* ✅ Convert both fields to lowercase
* ✅ Collapse multiple spaces/tabs/newlines into a single space
* ✅ Skip any row missing a required field
* ✅ Allow valid duplicates

---

## ✅ Interfaces to Implement

### 📄 `ISentimentDataLoader`

File: `SentimentAnalyzer.App/Contracts/ISentimentDataLoader.cs`

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using SentimentAnalyzer.App.Models;

namespace SentimentAnalyzer.App.Contracts
{
    public interface ISentimentDataLoader
    {
        /// <summary>
        /// Loads sentiment-labeled text data from a CSV file, normalizing and validating each record.
        /// </summary>
        /// <param name="filePath">Path to a UTF-8 encoded CSV file with at least "Sentiment" and "Text" columns</param>
        /// <returns>A read-only collection of <see cref="SentimentExample"/> objects</returns>
        Task<IReadOnlyCollection<SentimentExample>> LoadAsync(string filePath);
    }
}
```

---

### 📄 `SentimentExample` Model

File: `SentimentAnalyzer.App/Models/SentimentExample.cs`

```csharp
namespace SentimentAnalyzer.App.Models
{
    public class SentimentExample
    {
        public string Sentiment { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
```

---

## 🧪 Unit Test Coverage

File: `SentimentAnalyzer.Tests/PreprocessingTests/SentimentDataLoaderTests.cs`

This test suite validates correctness, robustness, and normalization logic for all expected scenarios.

| Test Name                          | Description                                                                   |
| ---------------------------------- | ----------------------------------------------------------------------------- |
| `Loads_Correct_Number_Of_Examples` | Parses known dataset and verifies record count                                |
| `Trims_And_Normalizes_Text`        | Confirms whitespace trimming and lowercasing                                  |
| `Handles_Empty_Lines_Gracefully`   | Ignores blank lines in CSV                                                    |
| `Skips_Rows_With_Missing_Fields`   | Skips rows missing `Sentiment` or `Text` fields                               |
| `Ignores_Extra_Columns`            | Validates that extra CSV columns are ignored                                  |
| `Allows_Duplicate_Entries`         | Confirms that duplicate rows are preserved                                    |
| `Throws_If_File_Does_Not_Exist`    | Throws `FileNotFoundException` if file is missing                             |
| `Normalizes_Whitespace_In_Text`    | Collapses internal whitespace (tabs/newlines/multiple spaces)                 |
| `Fails_On_Header_Mismatch`         | Throws `FormatException` if required headers are missing or incorrectly named |

Each test uses:

```csharp
var data = await _loader.LoadAsync(TestHelper.GetPath("stage1/your_file.csv"));
```

---

## ⚙️ Implementation Details

### 📄 `CsvSentimentDataLoader`

File: `SentimentAnalyzer.App/Preprocessing/CsvSentimentDataLoader.cs`

This class should:

1. Use `CsvReader` with the config above
2. Read all records into `SentimentExample`
3. Normalize each field using a helper (e.g. `TextNormalizer`)
4. Filter invalid rows
5. Return a `List<SentimentExample>` or `IReadOnlyCollection<>`

---


## 🧠 STAGE 2 — Model Training + Evaluation

### 🎯 Goal

Build a trainable ML.NET pipeline to:

1. **Load and process training data**
2. **Train a binary classifier**
3. **Evaluate model performance on a test set**
4. Return an object with evaluation metrics

---

## ✅ Interfaces to Implement

### 📄 `ISentimentModelTrainer`

`SentimentAnalyzer.App/Contracts/ISentimentModelTrainer.cs`:

```csharp
using System.Threading.Tasks;
using SentimentAnalyzer.App.Preprocessing;

namespace SentimentAnalyzer.App.Contracts
{
    public interface ISentimentModelTrainer
    {
        /// <summary>
        /// Trains a binary classification model on the provided training data and evaluates it on test data.
        /// </summary>
        /// <param name="trainPath">Path to the training dataset CSV</param>
        /// <param name="testPath">Path to the test dataset CSV</param>
        /// <returns>Model training metrics including Accuracy, AUC, and F1 Score</returns>
        Task<ModelTrainingResult> TrainAndEvaluateAsync(string trainPath, string testPath);
    }
}
```

---

### 📄 `ModelTrainingResult`

`SentimentAnalyzer.App/Preprocessing/ModelTrainingResult.cs`:

```csharp
namespace SentimentAnalyzer.App.Preprocessing
{
    public class ModelTrainingResult
    {
        public double Accuracy { get; set; }
        public double Auc { get; set; }
        public double F1Score { get; set; }
    }
}
```

---

## 🔧 ML.NET Concepts You'll Use

| Concept                    | Purpose                                          |
| -------------------------- | ------------------------------------------------ |
| `MLContext`                | Root entry point for all ML.NET operations       |
| `IDataView`                | Efficient, schema-aware data structure           |
| `LoadFromTextFile<T>()`    | Load CSV files into IDataView                    |
| `FeaturizeText()`          | Automatically tokenizes and vectorizes text      |
| `MapValueToKey()`          | Converts labels to key-type categories           |
| `SdcaLogisticRegression()` | Efficient binary classifier                      |
| `Evaluate()`               | Computes accuracy, AUC, and F1 score on test set |

---

## 📁 Data Files (in `data/stage2/`)

| File                       | Description         |
| -------------------------- | ------------------- |
| `train_sentiment_data.csv` | 70% of labeled data |
| `test_sentiment_data.csv`  | 30% of labeled data |

Each row looks like:

```csv
Sentiment,Text
Positive,"I loved this product!"
Negative,"Worst experience ever."
```

---

## 🧪 Unit Test Scaffold

Create in `SentimentAnalyzer.Tests/TrainingTests/ModelTrainerTests.cs`:

```csharp
using System.Threading.Tasks;
using SentimentAnalyzer.App.Contracts;
using SentimentAnalyzer.App.Preprocessing;
using Xunit;

namespace SentimentAnalyzer.Tests.TrainingTests
{
    public class ModelTrainerTests
    {
        private readonly ISentimentModelTrainer _trainer;

        public ModelTrainerTests()
        {
            _trainer = new SentimentModelTrainer(); // You'll implement this
        }

        private static string GetPath(string filename)
        {
            return Path.Combine(AppContext.BaseDirectory, "data", "stage2", filename);
        }

        [Fact]
        public async Task TrainAndEvaluate_ShouldReturnReasonableMetrics()
        {
            var result = await _trainer.TrainAndEvaluateAsync(
                GetPath("train_sentiment_data.csv"),
                GetPath("test_sentiment_data.csv")
            );

            Assert.InRange(result.Accuracy, 0.5, 1.0);
            Assert.InRange(result.Auc, 0.5, 1.0);
            Assert.InRange(result.F1Score, 0.5, 1.0);
        }
    }
}
```

---

## 🧠 Summary

| Task                 | You Implement                               |
| -------------------- | ------------------------------------------- |
| Load CSV → IDataView | `MLContext.Data.LoadFromTextFile<T>()`      |
| Build pipeline       | `MapValueToKey`, `FeaturizeText`, `Trainer` |
| Train model          | `Fit()`                                     |
| Evaluate model       | `Evaluate()` on test data                   |
| Return metrics       | Accuracy, AUC, F1                           |

---