using System.Text.Json;
using Xunit;
using System.Text.Json.Serialization;
namespace DomainCopilot.Tests;

public class GoldenDatasetTests
{
    private class GoldenQuestion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("question")]
        public string Question { get; set; } = string.Empty;

        [JsonPropertyName("expectedAnswer")]
        public string ExpectedAnswer { get; set; } = string.Empty;
    }

    [Fact]
    public void GoldenDataset_ShouldContain25Questions()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "GoldenDataset",
            "GoldenQuestions.json");

        Assert.True(
            File.Exists(filePath),
            "GoldenQuestions.json was not found.");

        var json = File.ReadAllText(filePath);

        var questions =
            JsonSerializer.Deserialize<List<GoldenQuestion>>(json);

        Assert.NotNull(questions);
        Assert.Equal(25, questions!.Count);
    }

    [Fact]
    public void GoldenDataset_ShouldContainAtLeast5AdversarialQuestions()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "GoldenDataset",
            "GoldenQuestions.json");

        var json = File.ReadAllText(filePath);

        var questions =
    JsonSerializer.Deserialize<List<GoldenQuestion>>(
        json,
        new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(questions);

        var adversarialCount =
            questions!.Count(q =>
                q.Category.Equals(
                    "Adversarial",
                    StringComparison.OrdinalIgnoreCase));

        Assert.True(
            adversarialCount >= 5,
            $"Expected at least 5 adversarial questions, but found {adversarialCount}.");
    }

    [Fact]
    public void GoldenDataset_ShouldHaveRequiredFields()
    {
        var filePath = Path.Combine(
            AppContext.BaseDirectory,
            "GoldenDataset",
            "GoldenQuestions.json");

        var json = File.ReadAllText(filePath);

        var questions =
            JsonSerializer.Deserialize<List<GoldenQuestion>>(json);

        Assert.NotNull(questions);

        Assert.All(questions!, question =>
        {
            Assert.True(question.Id > 0);
            Assert.False(string.IsNullOrWhiteSpace(question.Category));
            Assert.False(string.IsNullOrWhiteSpace(question.Question));
            Assert.False(string.IsNullOrWhiteSpace(question.ExpectedAnswer));
        });
    }
}