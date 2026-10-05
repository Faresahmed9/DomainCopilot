using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Infrastructure.Embeddings;

public class FakeEmbeddingService : IEmbeddingService
{
    public Task<IReadOnlyList<float>> GenerateDocumentEmbeddingAsync(
        string text)
    {
        return CreateFakeVector();
    }

    public Task<IReadOnlyList<float>> GenerateQueryEmbeddingAsync(
        string text)
    {
        return CreateFakeVector();
    }

    private static Task<IReadOnlyList<float>> CreateFakeVector()
    {
        var vector = new List<float>
        {
            0.1f,
            0.2f,
            0.3f
        };

        return Task.FromResult<IReadOnlyList<float>>(vector);
    }
}