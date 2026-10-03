using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class AnomalyRepository : IAnomalyRepository
{
    private readonly ApplicationDbContext _context;

    public AnomalyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Anomaly?> GetByIdAsync(Guid anomalyId)
    {
        return await _context.Anomalies
            .FirstOrDefaultAsync(a => a.AnomalyId == anomalyId);
    }

    public async Task<IReadOnlyList<Anomaly>> GetByClaimIdAsync(
        Guid claimId)
    {
        return await _context.Anomalies
            .Where(a => a.ClaimId == claimId)
            .ToListAsync();
    }

    public async Task AddAsync(Anomaly anomaly)
    {
        await _context.Anomalies.AddAsync(anomaly);
        await _context.SaveChangesAsync();
    }
}