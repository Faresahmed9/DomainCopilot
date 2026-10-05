namespace DomainCopilot.Application.Interfaces;

public interface IEmbeddingService
{
    Task<IReadOnlyList<float>> GenerateDocumentEmbeddingAsync(
        string text);

    Task<IReadOnlyList<float>> GenerateQueryEmbeddingAsync(
        string text);
}