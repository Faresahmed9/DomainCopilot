using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly GetClaimContextUseCase _getClaimContextUseCase;
    private readonly AdjudicateClaimUseCase _adjudicateClaimUseCase;

    public ClaimsController(
        GetClaimContextUseCase getClaimContextUseCase,
        AdjudicateClaimUseCase adjudicateClaimUseCase)
    {
        _getClaimContextUseCase = getClaimContextUseCase;
        _adjudicateClaimUseCase = adjudicateClaimUseCase;
    }

    [HttpGet("{claimId:guid}/context")]
    public async Task<IActionResult> GetContext(Guid claimId)
    {
        var result = await _getClaimContextUseCase
            .ExecuteAsync(claimId);

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
            .ExecuteAsync(claimId);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}