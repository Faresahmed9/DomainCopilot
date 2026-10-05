using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IDocumentRepository
{
    Task<Document?> GetByIdAsync(
        Guid documentId,
        Guid tenantId);

    Task AddAsync(Document document);

    Task UpdateAsync(Document document);
}