using DomainCopilot.Domain.Documents;

namespace DomainCopilot.Application.Interfaces;

public interface IDocumentChunkRepository
{
    Task AddAsync(DocumentChunk chunk);

    Task<IReadOnlyList<DocumentChunk>> GetByDocumentIdAsync(
        Guid documentId);

    Task<IReadOnlyList<DocumentChunk>> SearchAsync(
        Guid tenantId,
        string policyNumber,
        DateTime incidentDate);
}