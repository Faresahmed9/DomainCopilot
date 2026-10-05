using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;

namespace DomainCopilot.Infrastructure.Repositories;

public class AdjudicationDecisionRepository
    : IAdjudicationDecisionRepository
{
    private readonly ApplicationDbContext _context;

    public AdjudicationDecisionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(
        AdjudicationDecision decision)
    {
        await _context.AdjudicationDecisions.AddAsync(decision);
        await _context.SaveChangesAsync();
    }
}