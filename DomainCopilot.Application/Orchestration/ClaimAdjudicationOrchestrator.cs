using DomainCopilot.Application.Agents;
using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using DomainCopilot.Application.Services;
using DomainCopilot.Application.UseCases;
using DomainCopilot.Domain;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.Orchestration;

public class ClaimAdjudicationOrchestrator
{
    private readonly GetClaimContextUseCase _getClaimContextUseCase;
    private readonly RetrieveRelevantChunksUseCase _retrieveRelevantChunksUseCase;

    private readonly CoverageMatcherAgent _coverageMatcherAgent;
    private readonly ExclusionAnalystAgent _exclusionAnalystAgent;
    private readonly AdjudicationDrafterAgent _adjudicationDrafterAgent;

    private readonly CoverageEvaluator _coverageEvaluator;
    private readonly ExclusionEvaluator _exclusionEvaluator;
    private readonly ClaimAmountCalculator _claimAmountCalculator;
    private readonly AnomalyDetector _anomalyDetector;

    private readonly IAdjudicationRepository _adjudicationRepository;
    private readonly IApprovalRepository _approvalRepository;

    public ClaimAdjudicationOrchestrator(
        GetClaimContextUseCase getClaimContextUseCase,
        RetrieveRelevantChunksUseCase retrieveRelevantChunksUseCase,
        CoverageMatcherAgent coverageMatcherAgent,
        ExclusionAnalystAgent exclusionAnalystAgent,
        AdjudicationDrafterAgent adjudicationDrafterAgent,
        CoverageEvaluator coverageEvaluator,
        ExclusionEvaluator exclusionEvaluator,
        ClaimAmountCalculator claimAmountCalculator,
        AnomalyDetector anomalyDetector,
        IAdjudicationRepository adjudicationRepository,
        IApprovalRepository approvalRepository)
    {
        _getClaimContextUseCase = getClaimContextUseCase;
        _retrieveRelevantChunksUseCase = retrieveRelevantChunksUseCase;

        _coverageMatcherAgent = coverageMatcherAgent;
        _exclusionAnalystAgent = exclusionAnalystAgent;
        _adjudicationDrafterAgent = adjudicationDrafterAgent;

        _coverageEvaluator = coverageEvaluator;
        _exclusionEvaluator = exclusionEvaluator;
        _claimAmountCalculator = claimAmountCalculator;
        _anomalyDetector = anomalyDetector;

        _adjudicationRepository = adjudicationRepository;
        _approvalRepository = approvalRepository;
    }

    public async Task<OrchestrationResultDto?> ExecuteAsync(
        Guid claimId,
        Guid tenantId)
    {
        // 1. Get claim and the correct policy version
        var context = await _getClaimContextUseCase
            .ExecuteAsync(
                claimId,
                tenantId);

        if (context is null)
            return null;

        // 2. Retrieve relevant policy evidence using RAG
        var retrievalResults =
            await _retrieveRelevantChunksUseCase.ExecuteAsync(
                tenantId,
                context.Policy.PolicyNumber,
                context.Claim.IncidentDate,
                context.Claim.Description);

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

        // 5. Coverage Matcher Agent
        var coverageAnalysis =
            await _coverageMatcherAgent.MatchAsync(
                context.Claim.Description,
                policyEvidence);

        // 6. Exclusion Analyst Agent
        var exclusionAnalysis =
            await _exclusionAnalystAgent.AnalyzeAsync(
                context.Claim.Description,
                policyEvidence);

        // 7. Adjudication Drafter Agent
        var adjudicationDraft =
            await _adjudicationDrafterAgent.DraftAsync(
                context.Claim.Description,
                context.Claim.ClaimedAmount,
                coverageAnalysis,
                exclusionAnalysis,
                policyEvidence);

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
                _claimAmountCalculator.CalculateApprovedAmount(
                    context.Claim.ClaimedAmount,
                    matchingCoverage.Limit,
                    matchingCoverage.Deductible);

            decision =
                approvedAmount == context.Claim.ClaimedAmount
                    ? DecisionStatus.Approved
                    : DecisionStatus.PartiallyApproved;
        }

        // 9. Detect anomalies
        var anomalies =
            _anomalyDetector.Detect(
                context.Claim,
                matchingCoverage);

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

        // 12. Create Human Approval Request
        var approvalRequest =
            new ApprovalRequest(
                adjudicationDecision.AdjudicationDecisionId);

        await _approvalRepository.AddAsync(
            approvalRequest);

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