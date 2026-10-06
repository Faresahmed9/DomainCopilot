using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.UseCases;

public class GetDashboardSummaryUseCase
{
    private readonly IClaimRepository _claimRepository;
    private readonly IApprovalRequestRepository _approvalRequestRepository;

    public GetDashboardSummaryUseCase(
        IClaimRepository claimRepository,
        IApprovalRequestRepository approvalRequestRepository)
    {
        _claimRepository = claimRepository;
        _approvalRequestRepository = approvalRequestRepository;
    }

    public async Task<DashboardSummaryDto> ExecuteAsync(
        Guid tenantId)
    {
        var claims =
            await _claimRepository.GetAllAsync(tenantId);

        var approvals =
            await _approvalRequestRepository.GetPendingAsync(tenantId);

        return new DashboardSummaryDto
        {
            TotalClaims = claims.Count,
            PendingApprovals = approvals.Count
        };
    }
}