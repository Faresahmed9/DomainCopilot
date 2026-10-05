using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain.Documents;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class DocumentChunkRepository : IDocumentChunkRepository
{
    private readonly ApplicationDbContext _context;

    public DocumentChunkRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DocumentChunk chunk)
    {
        await _context.DocumentChunks.AddAsync(chunk);

        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<DocumentChunk>> GetByDocumentIdAsync(
        Guid documentId)
    {
        return await _context.DocumentChunks
            .Where(x => x.DocumentId == documentId)
            .OrderBy(x => x.ChunkIndex)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<DocumentChunk>> SearchAsync(
        Guid tenantId,
        string policyNumber,
        DateTime incidentDate)
    {
        return await (
            from chunk in _context.DocumentChunks
            join document in _context.Documents
                on chunk.DocumentId equals document.DocumentId
            where chunk.TenantId == tenantId
       && document.TenantId == tenantId
       && chunk.PolicyNumber == policyNumber
       && document.PolicyNumber == policyNumber
       && document.EffectiveFrom <= incidentDate
       && document.EffectiveTo >= incidentDate
            orderby chunk.PolicyVersion,
                    chunk.PageNumber,
                    chunk.ChunkIndex
            select chunk
        ).ToListAsync();
    }
}