using DomainCopilot.Application.Interfaces;
using Google.GenAI;
using Google.GenAI.Types;

namespace DomainCopilot.Infrastructure.Embeddings;

public class GeminiEmbeddingService : IEmbeddingService
{
    private readonly Client _client;

    public GeminiEmbeddingService(string apiKey)
    {
        _client = new Client(apiKey: apiKey);
    }

    public async Task<IReadOnlyList<float>> GenerateDocumentEmbeddingAsync(
        string text)
    {
        return await GenerateEmbeddingAsync(
            text,
            "RETRIEVAL_DOCUMENT");
    }

    public async Task<IReadOnlyList<float>> GenerateQueryEmbeddingAsync(
        string text)
    {
        return await GenerateEmbeddingAsync(
            text,
            "RETRIEVAL_QUERY");
    }

    private async Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
        string text,
        string taskType)
    {
        var response = await _client.Models.EmbedContentAsync(
            model: "gemini-embedding-001",
            contents: text,
            config: new EmbedContentConfig
            {
                TaskType = taskType,
                OutputDimensionality = 768
            });

        var embedding = response.Embeddings?.FirstOrDefault();

        if (embedding?.Values is null)
        {
            throw new InvalidOperationException(
                "Gemini did not return a valid embedding.");
        }

        return embedding.Values
            .Select(x => (float)x)
            .ToArray();
    }
}