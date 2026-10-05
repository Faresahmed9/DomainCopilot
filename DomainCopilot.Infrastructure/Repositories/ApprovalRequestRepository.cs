using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;
using DomainCopilot.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Repositories;

public class ApprovalRequestRepository : IApprovalRequestRepository
{
    private readonly ApplicationDbContext _context;


public ApprovalRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApprovalRequest?> GetByIdAsync(
        Guid approvalRequestId,
        Guid tenantId)
    {
        return await (
            from approval in _context.ApprovalRequests
            join decision in _context.AdjudicationDecisions
                on approval.AdjudicationDecisionId
                equals decision.AdjudicationDecisionId
            join claim in _context.Claims
                on decision.ClaimId
                equals claim.ClaimId
            where approval.ApprovalRequestId == approvalRequestId
                  && claim.TenantId == tenantId
            select approval
        ).FirstOrDefaultAsync();
    }

    public async Task AddAsync(ApprovalRequest approvalRequest)
    {
        await _context.ApprovalRequests.AddAsync(approvalRequest);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ApprovalRequest approvalRequest)
    {
        _context.ApprovalRequests.Update(approvalRequest);
        await _context.SaveChangesAsync();
    }


    public async Task<ApprovalRequest?> GetPendingByDecisionIdAsync(
    Guid adjudicationDecisionId,
    Guid tenantId)
    {
        return await (
            from approval in _context.ApprovalRequests
            join decision in _context.AdjudicationDecisions
                on approval.AdjudicationDecisionId
                equals decision.AdjudicationDecisionId
            join claim in _context.Claims
                on decision.ClaimId equals claim.ClaimId
            where decision.AdjudicationDecisionId == adjudicationDecisionId
                  && claim.TenantId == tenantId
                  && approval.Status == DomainCopilot.Domain.Enums.ApprovalStatus.Pending
            select approval
        ).FirstOrDefaultAsync();
    }


}
