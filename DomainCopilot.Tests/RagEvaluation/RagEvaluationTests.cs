using System.Text.Json;
using DomainCopilot.Application.Services;
using DomainCopilot.Application.UseCases;
using DomainCopilot.Infrastructure.Embeddings;
using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DomainCopilot.Tests;
[Trait("Category", "Integration")]
public class RagEvaluationTests
{
    [Fact]
    public async Task GoldenDataset_ShouldEvaluateRetrieval()
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new DocumentChunkRepository(context);

        var configuration = new ConfigurationBuilder()
    .AddUserSecrets(
        "77ff28f8-8c9f-41d7-8672-4a92b99d1569")
    .Build();

        var apiKey = configuration["Gemini:ApiKey"];

        Assert.False(
            string.IsNullOrWhiteSpace(apiKey),
            "GEMINI_API_KEY environment variable is missing.");

        var embeddingService =
            new GeminiEmbeddingService(apiKey!);

        var similarityCalculator =
            new CosineSimilarityCalculator();

        var keywordSearchService =
            new KeywordSearchService();

        var useCase =
            new RetrieveRelevantChunksUseCase(
                repository,
                embeddingService,
                similarityCalculator,
                keywordSearchService);

        var datasetPath =
            Path.Combine(
                AppContext.BaseDirectory,
                "GoldenDataset",
                "GoldenQuestions.json");

        Assert.True(
            File.Exists(datasetPath),
            $"Golden dataset was not found: {datasetPath}");

        var json =
            await File.ReadAllTextAsync(datasetPath);

        var questions =
            JsonSerializer.Deserialize<List<GoldenQuestion>>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        Assert.NotNull(questions);

        var mappings = CreateMappings();

        var evaluated = 0;
        var passed = 0;

        foreach (var question in questions!)
        {
            if (!mappings.TryGetValue(question.Id, out var mapping))
                continue;

            evaluated++;

            var results =
                await useCase.ExecuteAsync(
                    mapping.TenantId,
                    mapping.PolicyNumber,
                    mapping.IncidentDate,
                    question.Question);

            var found =
                results.Any(x =>
                    x.Content.Contains(
                        mapping.ExpectedAnswer,
                        StringComparison.OrdinalIgnoreCase));

            if (found)
            {
                passed++;

                Console.WriteLine(
                    $"PASS | {question.Id} | {question.Question}");
            }
            else
            {
                Console.WriteLine(
                    $"FAIL | {question.Id} | {question.Question}");
            }
        }

        Assert.True(
            evaluated > 0,
            "No Golden Questions were evaluated.");

        var accuracy =
            (double)passed / evaluated * 100;

        Console.WriteLine();
        Console.WriteLine(
            $"RAG Evaluation: {passed}/{evaluated} ({accuracy:F2}%)");

        Assert.True(
            accuracy >= 70,
            $"RAG retrieval accuracy was only {accuracy:F2}%.");
    }
    
    private static Dictionary<int, GoldenMapping> CreateMappings()
    {
        var tenantA =
            Guid.Parse(
                "A071EDBD-A4C8-4C53-81BB-9CEF1C72ECB2");

        return new Dictionary<int, GoldenMapping>
        {
            [1] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "Collision"),

            [2] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "Accidental vehicle damage"),

            [3] = new(
                tenantA,
                "POL-1013",
                new DateTime(2026, 6, 15),
                "Theft"),

            [4] = new(
                tenantA,
                "POL-1013",
                new DateTime(2026, 6, 15),
                "theft"),

            [5] = new(
                tenantA,
                "POL-1014",
                new DateTime(2026, 6, 15),
                "Fire"),

            [6] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "Intentional damage"),

            [7] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "Racing"),

            [8] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "driving under the influence"),

            [9] = new(
                tenantA,
                "POL-1015",
                new DateTime(2026, 6, 15),
                "No such exclusion"),

            [10] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "rejected"),

            [11] = new(
                tenantA,
                "POL-1001",
                new DateTime(2025, 6, 15),
                "Version 1"),

            [12] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "Version 2"),

            [13] = new(
                tenantA,
                "POL-1001",
                new DateTime(2025, 6, 15),
                "2025-01-01"),

            [14] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "2026-01-01"),

            [15] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "incident date"),

            [16] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "7000"),

            [17] = new(
                tenantA,
                "POL-1001",
                new DateTime(2026, 6, 15),
                "1000")
        };
    }

    private sealed record GoldenMapping(
        Guid TenantId,
        string PolicyNumber,
        DateTime IncidentDate,
        string ExpectedAnswer);

    private sealed class GoldenQuestion
    {
        public int Id { get; set; }

        public string Category { get; set; } = string.Empty;

        public string Question { get; set; } = string.Empty;

        public string ExpectedAnswer { get; set; } = string.Empty;
    }
}