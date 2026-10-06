using DomainCopilot.Application.Orchestration;
using DomainCopilot.Application.Tenant;
using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClaimsController : ControllerBase
{
    private readonly GetClaimContextUseCase _getClaimContextUseCase;
    private readonly AdjudicateClaimUseCase _adjudicateClaimUseCase;
    private readonly ClaimAdjudicationOrchestrator _claimAdjudicationOrchestrator;
    private readonly ITenantContext _tenantContext;

    public ClaimsController(
        GetClaimContextUseCase getClaimContextUseCase,
        AdjudicateClaimUseCase adjudicateClaimUseCase,
        ClaimAdjudicationOrchestrator claimAdjudicationOrchestrator,
        ITenantContext tenantContext)
    {
        _getClaimContextUseCase = getClaimContextUseCase;
        _adjudicateClaimUseCase = adjudicateClaimUseCase;
        _claimAdjudicationOrchestrator = claimAdjudicationOrchestrator;
        _tenantContext = tenantContext;
    }

    [HttpGet("{claimId:guid}/context")]
    public async Task<IActionResult> GetContext(Guid claimId)
    {
        var result = await _getClaimContextUseCase
            .ExecuteAsync(
                claimId,
                _tenantContext.TenantId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("{claimId:guid}/adjudicate")]
    public async Task<IActionResult> Adjudicate(Guid claimId)
    {
        var result = await _adjudicateClaimUseCase
            .ExecuteAsync(
                claimId,
                _tenantContext.TenantId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpPost("{claimId:guid}/orchestrate")]
    public async Task<IActionResult> Orchestrate(
      Guid claimId,
      CancellationToken cancellationToken)
    {
        var result = await _claimAdjudicationOrchestrator
            .ExecuteAsync(
                claimId,
                _tenantContext.TenantId,
                cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
    [HttpPost("{claimId:guid}/orchestrate/stream")]
    public async Task StreamOrchestration(
        Guid claimId,
        CancellationToken cancellationToken)
    {
        Response.ContentType = "text/event-stream";

        try
        {
            await Response.WriteAsync(
                "data: orchestration-started\n\n",
                cancellationToken);

            var progress = new Progress<string>(message =>
            {
                _ = Response.WriteAsync(
                    $"data: {message}\n\n",
                    cancellationToken);
            });

            var result =
                await _claimAdjudicationOrchestrator.ExecuteAsync(
                    claimId,
                    _tenantContext.TenantId,
                    cancellationToken,
                    progress);

            await Response.WriteAsync(
                "data: completed\n\n",
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Client cancelled the request.
        }
    }
}