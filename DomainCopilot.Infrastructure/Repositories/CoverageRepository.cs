using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class CoverageRepository : ICoverageRepository
{
    private readonly ApplicationDbContext _context;

    public CoverageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Coverage?> GetByIdAsync(Guid coverageId)
    {
        return await _context.Coverages
            .FirstOrDefaultAsync(c => c.CoverageId == coverageId);
    }

    public async Task<IReadOnlyList<Coverage>> GetByPolicyIdAsync(
        Guid policyId)
    {
        return await _context.Coverages
            .Where(c => c.PolicyId == policyId)
            .ToListAsync();
    }
}