using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.UseCases;

public class GenerateEmbeddingUseCase
{
    private readonly IEmbeddingService _embeddingService;

    public GenerateEmbeddingUseCase(
        IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<float>> ExecuteAsync(
        string text)
    {
        return await _embeddingService.GenerateDocumentEmbeddingAsync(text);
    }
}