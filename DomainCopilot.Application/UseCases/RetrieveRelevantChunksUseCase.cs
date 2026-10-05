using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using DomainCopilot.Application.Services;
using System.Text.Json;

namespace DomainCopilot.Application.UseCases;

public class RetrieveRelevantChunksUseCase
{
    private readonly IDocumentChunkRepository _documentChunkRepository;
    private readonly IEmbeddingService _embeddingService;
    private readonly CosineSimilarityCalculator _similarityCalculator;
    private readonly KeywordSearchService _keywordSearchService;

    public RetrieveRelevantChunksUseCase(
        IDocumentChunkRepository documentChunkRepository,
        IEmbeddingService embeddingService,
        CosineSimilarityCalculator similarityCalculator,
        KeywordSearchService keywordSearchService)
    {
        _documentChunkRepository = documentChunkRepository;
        _embeddingService = embeddingService;
        _similarityCalculator = similarityCalculator;
        _keywordSearchService = keywordSearchService;
    }

    public async Task<IReadOnlyList<RetrievedChunkDto>> ExecuteAsync(
        Guid tenantId,
        string policyNumber,
        DateTime incidentDate,
        string query)
    {
        var queryEmbedding =
            await _embeddingService.GenerateQueryEmbeddingAsync(query);

        var chunks =
            await _documentChunkRepository.SearchAsync(
                tenantId,
                policyNumber,
                incidentDate);

        var rankedChunks = chunks
            .Where(x => !string.IsNullOrWhiteSpace(x.EmbeddingJson))
            .Select(chunk =>
            {
                var chunkEmbedding =
                    JsonSerializer.Deserialize<List<float>>(
                        chunk.EmbeddingJson!);

                var denseScore =
                    chunkEmbedding is null
                        ? 0
                        : _similarityCalculator.Calculate(
                            queryEmbedding,
                            chunkEmbedding);

                var keywordScore =
                    _keywordSearchService.CalculateScore(
                        query,
                        chunk.Content);

                var combinedScore =
                    (denseScore * 0.7) +
                    (keywordScore * 0.3);

                return new
                {
                    Chunk = chunk,
                    CombinedScore = combinedScore
                };
            })
            .OrderByDescending(x => x.CombinedScore)
            .Take(5)
            .Select(x => new RetrievedChunkDto
            {
                DocumentChunkId = x.Chunk.DocumentChunkId,
                PolicyNumber = x.Chunk.PolicyNumber,
                PolicyVersion = x.Chunk.PolicyVersion,
                PageNumber = x.Chunk.PageNumber,
                Content = x.Chunk.Content,
                Score = x.CombinedScore
            })
            .ToList();

        return rankedChunks;
    }
}