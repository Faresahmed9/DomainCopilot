using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IClaimRepository
{
    Task<Claim?> GetByIdAsync(
        Guid claimId,
        Guid tenantId);

    Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        Guid tenantId);
}