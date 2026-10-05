using DomainCopilot.Application.Tenant;
using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Adjuster")]
public class ApprovalController : ControllerBase
{
    private readonly ApproveAdjudicationUseCase _approveAdjudicationUseCase;
    private readonly ITenantContext _tenantContext;

    public ApprovalController(
        ApproveAdjudicationUseCase approveAdjudicationUseCase,
        ITenantContext tenantContext)
    {
        _approveAdjudicationUseCase = approveAdjudicationUseCase;
        _tenantContext = tenantContext;
    }

    [HttpPost("{approvalRequestId:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid approvalRequestId,
        [FromBody] ReviewApprovalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewerId))
            return BadRequest("ReviewerId is required.");

        var result =
            await _approveAdjudicationUseCase.ExecuteAsync(
                approvalRequestId,
                _tenantContext.TenantId,
                request.ReviewerId,
                request.ReviewerComment);

        if (!result)
            return NotFound();

        return Ok(new
        {
            Message = "Adjudication approved successfully."
        });
    }

    [HttpPost("{approvalRequestId:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid approvalRequestId,
        [FromBody] ReviewApprovalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ReviewerId))
            return BadRequest("ReviewerId is required.");

        var result =
            await _approveAdjudicationUseCase.RejectAsync(
                approvalRequestId,
                _tenantContext.TenantId,
                request.ReviewerId,
                request.ReviewerComment);

        if (!result)
            return NotFound();

        return Ok(new
        {
            Message = "Adjudication rejected successfully."
        });
    }
}

public class ReviewApprovalRequest
{
    public string ReviewerId { get; set; } = string.Empty;

    public string ReviewerComment { get; set; } = string.Empty;
}