using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class ClaimRepository : IClaimRepository
{
    private readonly ApplicationDbContext _context;

    public ClaimRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Claim?> GetByIdAsync(Guid claimId)
    {
        return await _context.Claims
            .FirstOrDefaultAsync(c => c.ClaimId == claimId);
    }

    public async Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        Guid tenantId)
    {
        return await _context.Claims
            .FirstOrDefaultAsync(c =>
                c.ClaimNumber == claimNumber &&
                c.TenantId == tenantId);
    }
}