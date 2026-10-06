using DomainCopilot.Application.Tenant;
using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly GetDashboardSummaryUseCase _getDashboardSummaryUseCase;
    private readonly ITenantContext _tenantContext;

    public DashboardController(
        GetDashboardSummaryUseCase getDashboardSummaryUseCase,
        ITenantContext tenantContext)
    {
        _getDashboardSummaryUseCase = getDashboardSummaryUseCase;
        _tenantContext = tenantContext;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result =
            await _getDashboardSummaryUseCase.ExecuteAsync(
                _tenantContext.TenantId);

        return Ok(result);
    }
}