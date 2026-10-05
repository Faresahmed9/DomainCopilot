using DomainCopilot.Application.Agents;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly CoverageMatcherAgent _coverageMatcherAgent;
    private readonly ExclusionAnalystAgent _exclusionAnalystAgent;
    private readonly AdjudicationDrafterAgent _adjudicationDrafterAgent;

    public AgentsController(
    CoverageMatcherAgent coverageMatcherAgent,
    ExclusionAnalystAgent exclusionAnalystAgent,
    AdjudicationDrafterAgent adjudicationDrafterAgent)
    {
        _coverageMatcherAgent = coverageMatcherAgent;
        _exclusionAnalystAgent = exclusionAnalystAgent;
        _adjudicationDrafterAgent = adjudicationDrafterAgent;
    }

    [HttpPost("coverage-match")]
    public async Task<IActionResult> CoverageMatch(
        [FromBody] CoverageMatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClaimDescription))
            return BadRequest("Claim description is required.");

        if (string.IsNullOrWhiteSpace(request.PolicyEvidence))
            return BadRequest("Policy evidence is required.");

        var result = await _coverageMatcherAgent.MatchAsync(
            request.ClaimDescription,
            request.PolicyEvidence);

        return Ok(new
        {
            result
        });
    }

    [HttpPost("exclusion-analysis")]
    public async Task<IActionResult> ExclusionAnalysis(
        [FromBody] ExclusionAnalysisRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClaimDescription))
            return BadRequest("Claim description is required.");

        if (string.IsNullOrWhiteSpace(request.PolicyEvidence))
            return BadRequest("Policy evidence is required.");

        var result = await _exclusionAnalystAgent.AnalyzeAsync(
            request.ClaimDescription,
            request.PolicyEvidence);

        return Ok(new
        {
            result
        });
    }

    [HttpPost("adjudication-draft")]
    public async Task<IActionResult> AdjudicationDraft(
    [FromBody] AdjudicationDraftRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ClaimDescription))
            return BadRequest("Claim description is required.");

        if (request.ClaimedAmount <= 0)
            return BadRequest("Claimed amount must be greater than zero.");

        if (string.IsNullOrWhiteSpace(request.CoverageAnalysis))
            return BadRequest("Coverage analysis is required.");

        if (string.IsNullOrWhiteSpace(request.ExclusionAnalysis))
            return BadRequest("Exclusion analysis is required.");

        if (string.IsNullOrWhiteSpace(request.PolicyEvidence))
            return BadRequest("Policy evidence is required.");

        var result = await _adjudicationDrafterAgent.DraftAsync(
            request.ClaimDescription,
            request.ClaimedAmount,
            request.CoverageAnalysis,
            request.ExclusionAnalysis,
            request.PolicyEvidence);

        return Ok(new
        {
            result
        });
    }
}

public class CoverageMatchRequest
{
    public string ClaimDescription { get; set; }
    public string PolicyEvidence { get; set; }
}

public class ExclusionAnalysisRequest
{
    public string ClaimDescription { get; set; }
    public string PolicyEvidence { get; set; }
}


public class AdjudicationDraftRequest
{
    public string ClaimDescription { get; set; }
    public decimal ClaimedAmount { get; set; }
    public string CoverageAnalysis { get; set; }
    public string ExclusionAnalysis { get; set; }
    public string PolicyEvidence { get; set; }
}