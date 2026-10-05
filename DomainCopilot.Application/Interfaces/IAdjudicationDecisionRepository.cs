using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IAdjudicationDecisionRepository
{
    Task AddAsync(AdjudicationDecision decision);
}