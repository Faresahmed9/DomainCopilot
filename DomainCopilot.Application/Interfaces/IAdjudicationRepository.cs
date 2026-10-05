using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces
{
    // مسؤول عن تخزين واسترجاع الـ Decision، مش عن اتخاذ القرار نفسه
    public interface IAdjudicationRepository
    {
        Task<AdjudicationDecision?> GetByIdAsync(
            Guid adjudicationDecisionId,
            Guid tenantId);

        Task<AdjudicationDecision?> GetByClaimIdAsync(
            Guid claimId,
            Guid tenantId);

        Task AddAsync(
            AdjudicationDecision decision);
    }
}