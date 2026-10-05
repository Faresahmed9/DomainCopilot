using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.UseCases;

public class ApproveAdjudicationUseCase
{
    private readonly IApprovalRepository _approvalRepository;

    public ApproveAdjudicationUseCase(
        IApprovalRepository approvalRepository)
    {
        _approvalRepository = approvalRepository;
    }

    public async Task<bool> ExecuteAsync(
        Guid approvalRequestId,
        Guid tenantId,
        string reviewerId,
        string reviewerComment)
    {
        var approvalRequest =
            await _approvalRepository.GetByIdAsync(
                approvalRequestId,
                tenantId);

        if (approvalRequest is null)
            return false;

        if (approvalRequest.Status != ApprovalStatus.Pending)
            return false;

        approvalRequest.Approve(
            reviewerId,
            reviewerComment);

        await _approvalRepository.UpdateAsync(
            approvalRequest);

        return true;
    }

    public async Task<bool> RejectAsync(
        Guid approvalRequestId,
        Guid tenantId,
        string reviewerId,
        string reviewerComment)
    {
        var approvalRequest =
            await _approvalRepository.GetByIdAsync(
                approvalRequestId,
                tenantId);

        if (approvalRequest is null)
            return false;

        if (approvalRequest.Status != ApprovalStatus.Pending)
            return false;

        approvalRequest.Reject(
            reviewerId,
            reviewerComment);

        await _approvalRepository.UpdateAsync(
            approvalRequest);

        return true;
    }
}