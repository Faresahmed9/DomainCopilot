using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IApprovalRequestRepository
{
    Task<ApprovalRequest?> GetByIdAsync(
        Guid approvalRequestId,
        Guid tenantId);

    Task<ApprovalRequest?> GetPendingByDecisionIdAsync(
        Guid adjudicationDecisionId,
        Guid tenantId);

    Task<IReadOnlyList<ApprovalRequest>> GetPendingAsync(
        Guid tenantId);

    Task AddAsync(ApprovalRequest approvalRequest);

    Task UpdateAsync(ApprovalRequest approvalRequest);
}