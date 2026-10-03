using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class AdjudicationRepository : IAdjudicationRepository
{
    private readonly ApplicationDbContext _context;

    public AdjudicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdjudicationDecision?> GetByIdAsync(
        Guid adjudicationDecisionId)
    {
        return await _context.AdjudicationDecisions
            .FirstOrDefaultAsync(d =>
                d.AdjudicationDecisionId == adjudicationDecisionId);
    }

    public async Task<AdjudicationDecision?> GetByClaimIdAsync(
        Guid claimId)
    {
        return await _context.AdjudicationDecisions
            .FirstOrDefaultAsync(d => d.ClaimId == claimId);
    }

    public async Task AddAsync(
        AdjudicationDecision decision)
    {
        await _context.AdjudicationDecisions.AddAsync(decision);
        await _context.SaveChangesAsync();
    }
}