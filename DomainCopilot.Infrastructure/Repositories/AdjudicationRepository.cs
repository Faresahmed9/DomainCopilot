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
        Guid adjudicationDecisionId,
        Guid tenantId)
    {
        return await (
            from decision in _context.AdjudicationDecisions
            join claim in _context.Claims
                on decision.ClaimId equals claim.ClaimId
            where decision.AdjudicationDecisionId == adjudicationDecisionId
                  && claim.TenantId == tenantId
            select decision
        ).FirstOrDefaultAsync();
    }

    public async Task<AdjudicationDecision?> GetByClaimIdAsync(
        Guid claimId,
        Guid tenantId)
    {
        return await (
            from decision in _context.AdjudicationDecisions
            join claim in _context.Claims
                on decision.ClaimId equals claim.ClaimId
            where decision.ClaimId == claimId
                  && claim.TenantId == tenantId
            select decision
        ).FirstOrDefaultAsync();
    }

    public async Task AddAsync(
        AdjudicationDecision decision)
    {
        await _context.AdjudicationDecisions.AddAsync(decision);

        await _context.SaveChangesAsync();
    }
}