using DomainCopilot.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using DomainCopilot.Application.Tenant;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RetrievalController : ControllerBase
{
    private readonly RetrieveRelevantChunksUseCase _retrieveRelevantChunksUseCase;
    private readonly ITenantContext _tenantContext;

    public RetrievalController(
     RetrieveRelevantChunksUseCase retrieveRelevantChunksUseCase,
     ITenantContext tenantContext)
    {
        _retrieveRelevantChunksUseCase =
            retrieveRelevantChunksUseCase;

        _tenantContext = tenantContext;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search(
        [FromBody] RetrievalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
        {
            return BadRequest("Query is required.");
        }

        var results =
            await _retrieveRelevantChunksUseCase.ExecuteAsync(
                _tenantContext.TenantId,
                request.PolicyNumber,
                request.IncidentDate,
                request.Query);

        return Ok(results);
    }
}

public class RetrievalRequest
{
    public string PolicyNumber { get; set; }
    public DateTime IncidentDate { get; set; }
    public string Query { get; set; }
}