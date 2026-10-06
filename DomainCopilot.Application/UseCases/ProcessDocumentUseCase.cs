using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.UseCases;

public class ProcessDocumentUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorage _documentStorage;
    private readonly IDocumentTextExtractor _textExtractor;
    private readonly IChunkingService _chunkingService;
    private readonly IDocumentChunkRepository _documentChunkRepository;
    private readonly ITextCleaner _textCleaner;
    private readonly IEmbeddingService _embeddingService;

    public ProcessDocumentUseCase(
        IDocumentRepository documentRepository,
        IDocumentStorage documentStorage,
        IDocumentTextExtractor textExtractor,
        IChunkingService chunkingService,
        IDocumentChunkRepository documentChunkRepository,
        ITextCleaner textCleaner,
        IEmbeddingService embeddingService)
    {
        _documentRepository = documentRepository;
        _documentStorage = documentStorage;
        _textExtractor = textExtractor;
        _chunkingService = chunkingService;
        _documentChunkRepository = documentChunkRepository;
        _textCleaner = textCleaner;
        _embeddingService = embeddingService;
    }

    public async Task<IReadOnlyList<Guid>?> ExecuteAsync(Guid documentId, Guid tenantId)
    {
        var document =
            await _documentRepository.GetByIdAsync(documentId,tenantId);

        if (document is null)
            return null;

        await _documentChunkRepository.DeleteByDocumentIdAsync(
        documentId,
        tenantId);

        await using var fileStream =
            await _documentStorage.OpenReadAsync(
                document.StoragePath);

        var pages =
            await _textExtractor.ExtractPagesAsync(
                fileStream,
                document.ContentType);

        var allChunks = new List<DomainCopilot.Domain.Documents.DocumentChunk>();

        foreach (var page in pages)
        {
            var cleanedText =
                _textCleaner.Clean(page.Content);

            var chunks =
                _chunkingService.CreateChunks(
                    documentId,
                    document.TenantId,
                    document.PolicyNumber,
                    document.PolicyVersion,
                    page.PageNumber,
                    cleanedText);

            allChunks.AddRange(chunks);
        }

        foreach (var chunk in allChunks)
        {
            var embedding =
                await _embeddingService.GenerateDocumentEmbeddingAsync(
                    chunk.Content);

            chunk.SetEmbedding(
                System.Text.Json.JsonSerializer.Serialize(embedding));

            await _documentChunkRepository.AddAsync(chunk);
        }

        return allChunks
            .Select(x => x.DocumentChunkId)
            .ToList();
    }
}
