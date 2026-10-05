using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IPolicyRepository
{
    Task<Policy?> GetByIdAsync(
        Guid policyId,
        Guid tenantId);

    Task<Policy?> GetActiveVersionAsync(
        string policyNumber,
        DateTime incidentDate,
        Guid tenantId);
}