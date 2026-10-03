using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class ExclusionRepository : IExclusionRepository
{
    private readonly ApplicationDbContext _context;

    public ExclusionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Exclusion?> GetByIdAsync(Guid exclusionId)
    {
        return await _context.Exclusions
            .FirstOrDefaultAsync(e => e.ExclusionId == exclusionId);
    }

    public async Task<IReadOnlyList<Exclusion>> GetByPolicyIdAsync(
        Guid policyId)
    {
        return await _context.Exclusions
            .Where(e => e.PolicyId == policyId)
            .ToListAsync();
    }
}