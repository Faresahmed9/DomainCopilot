using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class ApprovalRepository : IApprovalRepository
{
    private readonly ApplicationDbContext _context;

    public ApprovalRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalRequest?> GetByIdAsync(
        Guid approvalRequestId)
    {
        return await _context.ApprovalRequests
            .FirstOrDefaultAsync(a =>
                a.ApprovalRequestId == approvalRequestId);
    }

    public async Task<ApprovalRequest?> GetByDecisionIdAsync(
        Guid adjudicationDecisionId)
    {
        return await _context.ApprovalRequests
            .FirstOrDefaultAsync(a =>
                a.AdjudicationDecisionId == adjudicationDecisionId);
    }

    public async Task AddAsync(
        ApprovalRequest approvalRequest)
    {
        await _context.ApprovalRequests
            .AddAsync(approvalRequest);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        ApprovalRequest approvalRequest)
    {
        _context.ApprovalRequests.Update(approvalRequest);

        await _context.SaveChangesAsync();
    }
}