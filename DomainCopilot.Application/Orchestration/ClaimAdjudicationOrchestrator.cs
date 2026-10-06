
using DomainCopilot.Application.Agents;
using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using DomainCopilot.Application.Services;
using DomainCopilot.Application.Tools;
using DomainCopilot.Domain;
using DomainCopilot.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace DomainCopilot.Application.Orchestration;

public class ClaimAdjudicationOrchestrator
{
    private readonly ClaimContextTool _claimContextTool;
    private readonly PolicyRetrievalTool _policyRetrievalTool;

    private readonly CoverageMatcherAgent _coverageMatcherAgent;
    private readonly ExclusionAnalystAgent _exclusionAnalystAgent;
    private readonly AdjudicationDrafterAgent _adjudicationDrafterAgent;

    private readonly CoverageEvaluator _coverageEvaluator;
    private readonly ExclusionEvaluator _exclusionEvaluator;
    private readonly ClaimAmountCalculatorTool _claimAmountCalculatorTool;
    private readonly AnomalyDetectionTool _anomalyDetectionTool;

    private readonly IAdjudicationRepository _adjudicationRepository;
    private readonly IApprovalRepository _approvalRepository;

    private readonly ILogger<ClaimAdjudicationOrchestrator> _logger;

    public ClaimAdjudicationOrchestrator(
        ClaimContextTool claimContextTool,
        PolicyRetrievalTool policyRetrievalTool,
        CoverageMatcherAgent coverageMatcherAgent,
        ExclusionAnalystAgent exclusionAnalystAgent,
        AdjudicationDrafterAgent adjudicationDrafterAgent,
        CoverageEvaluator coverageEvaluator,
        ExclusionEvaluator exclusionEvaluator,
        ClaimAmountCalculatorTool claimAmountCalculatorTool,
        AnomalyDetectionTool anomalyDetectionTool,
        IAdjudicationRepository adjudicationRepository,
        IApprovalRepository approvalRepository,
        ILogger<ClaimAdjudicationOrchestrator> logger)
    {
        _claimContextTool = claimContextTool;
        _policyRetrievalTool = policyRetrievalTool;

        _coverageMatcherAgent = coverageMatcherAgent;
        _exclusionAnalystAgent = exclusionAnalystAgent;
        _adjudicationDrafterAgent = adjudicationDrafterAgent;

        _coverageEvaluator = coverageEvaluator;
        _exclusionEvaluator = exclusionEvaluator;
        _claimAmountCalculatorTool = claimAmountCalculatorTool;
        _anomalyDetectionTool = anomalyDetectionTool;

        _adjudicationRepository = adjudicationRepository;
        _approvalRepository = approvalRepository;

        _logger = logger;
    }

    public async Task<OrchestrationResultDto?> ExecuteAsync(
        Guid claimId,
        Guid tenantId,
        CancellationToken cancellationToken = default,
        IProgress<string>? progress = null)
    {
        _logger.LogInformation(
            "Starting claim adjudication orchestration. ClaimId: {ClaimId}, TenantId: {TenantId}",
            claimId,
            tenantId);

        progress?.Report("Loading claim context.");

        // 1. Get claim and the correct policy version
        var context = await _claimContextTool
            .ExecuteAsync(
                claimId,
                tenantId);

        if (context is null)
            return null;

        _logger.LogInformation(
            "Claim context loaded. ClaimId: {ClaimId}, PolicyNumber: {PolicyNumber}, PolicyVersion: {PolicyVersion}",
            claimId,
            context.Policy.PolicyNumber,
            context.Policy.Version);

        progress?.Report("Claim context loaded.");

        cancellationToken.ThrowIfCancellationRequested();

        // 2. Retrieve relevant policy evidence using RAG
        var retrievalResults =
            await _policyRetrievalTool.ExecuteAsync(
                tenantId,
                context.Policy.PolicyNumber,
                context.Claim.IncidentDate,
                context.Claim.Description);

        _logger.LogInformation(
            "Policy retrieval completed. ClaimId: {ClaimId}, RetrievedChunks: {Count}",
            claimId,
            retrievalResults.Count);

        progress?.Report("Policy evidence retrieved.");

        cancellationToken.ThrowIfCancellationRequested();

        var policyEvidence = string.Join(
            "\n\n",
            retrievalResults.Select(x =>
                $"Policy Number: {x.PolicyNumber}\n" +
                $"Policy Version: {x.PolicyVersion}\n" +
                $"Page: {x.PageNumber}\n" +
                $"Content: {x.Content}"));

        // 3. Deterministic coverage evaluation
        var matchingCoverage =
            _coverageEvaluator.FindMatchingCoverage(
                context.Claim,
                context.Coverages);

        // 4. Deterministic exclusion evaluation
        var matchingExclusion =
            _exclusionEvaluator.FindMatchingExclusion(
                context.Claim,
                context.Exclusions);

        _logger.LogInformation(
            "Deterministic policy evaluation completed. ClaimId: {ClaimId}, HasCoverage: {HasCoverage}, HasExclusion: {HasExclusion}",
            claimId,
            matchingCoverage is not null,
            matchingExclusion is not null);

        progress?.Report("Policy coverage and exclusions evaluated.");

        cancellationToken.ThrowIfCancellationRequested();

        // 5. Coverage Matcher Agent
        _logger.LogInformation(
            "Starting specialist agent analysis. ClaimId: {ClaimId}",
            claimId);

        var coverageAnalysis =
            await _coverageMatcherAgent.MatchAsync(
                context.Claim.Description,
                policyEvidence);

        cancellationToken.ThrowIfCancellationRequested();

        // 6. Exclusion Analyst Agent
        var exclusionAnalysis =
            await _exclusionAnalystAgent.AnalyzeAsync(
                context.Claim.Description,
                policyEvidence);

        cancellationToken.ThrowIfCancellationRequested();

        // 7. Adjudication Drafter Agent
        var adjudicationDraft =
            await _adjudicationDrafterAgent.DraftAsync(
                context.Claim.Description,
                context.Claim.ClaimedAmount,
                coverageAnalysis,
                exclusionAnalysis,
                policyEvidence);

        _logger.LogInformation(
            "Specialist agent analysis completed. ClaimId: {ClaimId}",
            claimId);

        progress?.Report("AI analysis completed.");

        cancellationToken.ThrowIfCancellationRequested();

        // 8. Deterministic financial calculation
        decimal approvedAmount = 0;
        DecisionStatus decision;

        if (matchingCoverage is null)
        {
            decision = DecisionStatus.Rejected;
        }
        else if (matchingExclusion is not null)
        {
            decision = DecisionStatus.Rejected;
        }
        else
        {
            approvedAmount =
                _claimAmountCalculatorTool.Execute(
                    context.Claim.ClaimedAmount,
                    matchingCoverage.Limit,
                    matchingCoverage.Deductible);

            decision =
                approvedAmount == context.Claim.ClaimedAmount
                    ? DecisionStatus.Approved
                    : DecisionStatus.PartiallyApproved;
        }

        _logger.LogInformation(
            "Deterministic adjudication completed. ClaimId: {ClaimId}, Decision: {Decision}, ApprovedAmount: {ApprovedAmount}",
            claimId,
            decision,
            approvedAmount);

        progress?.Report("Deterministic financial calculation completed.");

        cancellationToken.ThrowIfCancellationRequested();

        // 9. Detect anomalies
        var anomalies =
            _anomalyDetectionTool.Execute(
                context.Claim,
                matchingCoverage);

        progress?.Report("Anomaly detection completed.");

        cancellationToken.ThrowIfCancellationRequested();

        // 10. Build decision reason
        var reason =
            matchingCoverage is null
                ? "No matching coverage was found."
                : matchingExclusion is not null
                    ? "The claim matches a policy exclusion."
                    : "Claim is covered and the approved amount was calculated deterministically using the deductible and coverage limit.";

        // 11. Save adjudication decision
        var adjudicationDecision =
            new AdjudicationDecision(
                context.Claim.ClaimId,
                decision,
                approvedAmount,
                reason);

        await _adjudicationRepository.AddAsync(
            adjudicationDecision);

        progress?.Report("Adjudication decision created.");

        cancellationToken.ThrowIfCancellationRequested();

        // 12. Create Human Approval Request
        var approvalRequest =
            new ApprovalRequest(
                adjudicationDecision.AdjudicationDecisionId);

        await _approvalRepository.AddAsync(
            approvalRequest);

        _logger.LogInformation(
            "Human approval request created. ClaimId: {ClaimId}, ApprovalRequestId: {ApprovalRequestId}",
            claimId,
            approvalRequest.ApprovalRequestId);

        progress?.Report(
            "Human approval request created and is pending.");

        _logger.LogInformation(
            "Claim adjudication orchestration completed. ClaimId: {ClaimId}",
            claimId);

        progress?.Report(
            "Adjudication orchestration completed.");

        // 13. Return the complete recommendation
        return new OrchestrationResultDto
        {
            ClaimId = context.Claim.ClaimId,
            PolicyId = context.Policy.PolicyId,
            PolicyVersion = context.Policy.Version,

            Decision = decision,

            ClaimedAmount = context.Claim.ClaimedAmount,
            ApprovedAmount = approvedAmount,

            Deductible = matchingCoverage?.Deductible ?? 0,
            CoverageLimit = matchingCoverage?.Limit ?? 0,

            HasExclusion = matchingExclusion is not null,

            CoverageAnalysis = coverageAnalysis,
            ExclusionAnalysis = exclusionAnalysis,
            AdjudicationDraft = adjudicationDraft,

            RetrievedChunks = retrievalResults,
            Anomalies = anomalies,

            ApprovalRequestId =
                approvalRequest.ApprovalRequestId
        };
    }
}

