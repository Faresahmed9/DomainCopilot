using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces
{
    // مسؤول عن طلبات موافقة الـ Adjuster.
    public interface IApprovalRepository
    {
        Task<ApprovalRequest?> GetByIdAsync(
            Guid approvalRequestId,
            Guid tenantId);

        Task<ApprovalRequest?> GetByDecisionIdAsync(
            Guid adjudicationDecisionId,
            Guid tenantId);

        Task AddAsync(
            ApprovalRequest approvalRequest);

        Task UpdateAsync(
            ApprovalRequest approvalRequest);
    }
}