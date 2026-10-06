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

    public async Task<Claim?> GetByIdAsync(
        Guid claimId,
        Guid tenantId)
    {
        return await _context.Claims
            .FirstOrDefaultAsync(x =>
                x.ClaimId == claimId &&
                x.TenantId == tenantId);
    }

    public async Task<Claim?> GetByClaimNumberAsync(
        string claimNumber,
        Guid tenantId)
    {
        return await _context.Claims
            .FirstOrDefaultAsync(x =>
                x.ClaimNumber == claimNumber &&
                x.TenantId == tenantId);
    }

    public async Task<IReadOnlyList<Claim>> GetAllAsync(
    Guid tenantId)
    {
        return await _context.Claims
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.ClaimId)
            .ToListAsync();
    }
}