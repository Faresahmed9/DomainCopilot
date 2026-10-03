using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using DomainCopilot.Infrastructure.Persistence;

namespace DomainCopilot.Infrastructure.Repositories;

public class PolicyRepository : IPolicyRepository
{
    private readonly ApplicationDbContext _context;

    public PolicyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Policy?> GetByIdAsync(Guid policyId)
    {
        return await _context.Policies
            .FirstOrDefaultAsync(p => p.PolicyId == policyId);
    }
    // هيجيب الـ Version اللي كانت سارية وقت وقوع الحادث.
    public async Task<Policy?> GetActiveVersionAsync(
        string policyNumber,
        DateTime incidentDate)
    {
        return await _context.Policies
            .Where(p =>
                p.PolicyNumber == policyNumber &&
                p.EffectiveFrom <= incidentDate &&
                p.EffectiveTo >= incidentDate)
            .OrderByDescending(p => p.Version)
            .FirstOrDefaultAsync();
    }
}