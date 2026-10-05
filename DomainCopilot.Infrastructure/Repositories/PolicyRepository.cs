using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class PolicyRepository : IPolicyRepository
{
    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Policy?> GetByIdAsync(
        Guid policyId,
        Guid tenantId)
    {
        return await _context.Policies
            .FirstOrDefaultAsync(x =>
                x.PolicyId == policyId &&
                x.TenantId == tenantId);
    }

    public async Task<Policy?> GetActiveVersionAsync(
        string policyNumber,
        DateTime incidentDate,
        Guid tenantId)
    {
        return await _context.Policies
            .Where(x =>
                x.TenantId == tenantId &&
                x.PolicyNumber == policyNumber &&
                x.EffectiveFrom <= incidentDate &&
                x.EffectiveTo >= incidentDate)
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync();
    }
}