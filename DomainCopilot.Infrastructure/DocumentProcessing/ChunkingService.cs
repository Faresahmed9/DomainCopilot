using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain.Documents;

namespace DomainCopilot.Infrastructure.DocumentProcessing;

public class ChunkingService : IChunkingService
{
    private const int ChunkSize = 1000;
    private const int Overlap = 200;

    public IReadOnlyList<DocumentChunk> CreateChunks(
        Guid documentId,
        Guid tenantId,
        string policyNumber,
        int policyVersion,
        int pageNumber,
        string text)
    {
        var chunks = new List<DocumentChunk>();

        if (string.IsNullOrWhiteSpace(text))
            return chunks;

        var startIndex = 0;
        var chunkIndex = 0;

        while (startIndex < text.Length)
        {
            var length = Math.Min(
                ChunkSize,
                text.Length - startIndex);

            var chunkText = text.Substring(
                startIndex,
                length);

            chunks.Add(
                new DocumentChunk(
                    documentId,
                    tenantId,
                    policyNumber,
                    policyVersion,
                    pageNumber,
                    chunkIndex,
                    chunkText));

            chunkIndex++;

            if (startIndex + length >= text.Length)
                break;

            startIndex += ChunkSize - Overlap;
        }

        return chunks;
    }
}