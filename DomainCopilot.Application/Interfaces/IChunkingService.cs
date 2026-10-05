
using DomainCopilot.Domain.Documents;

namespace DomainCopilot.Application.Interfaces;

public interface IChunkingService
{
    IReadOnlyList<DocumentChunk> CreateChunks(
        Guid documentId,
        Guid tenantId,
        string policyNumber,
        int policyVersion,
        int pageNumber,
        string text);
}

