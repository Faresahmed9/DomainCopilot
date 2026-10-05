using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Services;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.UseCases;

public class AdjudicateClaimUseCase
{
    private readonly GetClaimContextUseCase _getClaimContextUseCase;
    private readonly CoverageEvaluator _coverageEvaluator;
    private readonly ExclusionEvaluator _exclusionEvaluator;
    private readonly ClaimAmountCalculator _claimAmountCalculator;

    public AdjudicateClaimUseCase(
        GetClaimContextUseCase getClaimContextUseCase,
        CoverageEvaluator coverageEvaluator,
        ExclusionEvaluator exclusionEvaluator,
        ClaimAmountCalculator claimAmountCalculator)
    {
        _getClaimContextUseCase = getClaimContextUseCase;
        _coverageEvaluator = coverageEvaluator;
        _exclusionEvaluator = exclusionEvaluator;
        _claimAmountCalculator = claimAmountCalculator;
    }

    public async Task<AdjudicationResultDto?> ExecuteAsync(
        Guid claimId,
        Guid tenantId)
    {
        var context = await _getClaimContextUseCase
            .ExecuteAsync(
                claimId,
                tenantId);

        if (context is null)
        {
            return null;
        }

        var matchingCoverage =
            _coverageEvaluator.FindMatchingCoverage(
                context.Claim,
                context.Coverages);

        var matchingExclusion =
            _exclusionEvaluator.FindMatchingExclusion(
                context.Claim,
                context.Exclusions);

        if (matchingCoverage is null)
        {
            return new AdjudicationResultDto
            {
                ClaimId = context.Claim.ClaimId,
                PolicyId = context.Policy.PolicyId,
                PolicyVersion = context.Policy.Version,
                Decision = DecisionStatus.Rejected,
                ClaimedAmount = context.Claim.ClaimedAmount,
                ApprovedAmount = 0,
                HasExclusion = matchingExclusion is not null,
                Reason = "No matching coverage was found."
            };
        }

        if (matchingExclusion is not null)
        {
            return new AdjudicationResultDto
            {
                ClaimId = context.Claim.ClaimId,
                PolicyId = context.Policy.PolicyId,
                PolicyVersion = context.Policy.Version,
                Decision = DecisionStatus.Rejected,
                ClaimedAmount = context.Claim.ClaimedAmount,
                ApprovedAmount = 0,
                Deductible = matchingCoverage.Deductible,
                CoverageLimit = matchingCoverage.Limit,
                HasExclusion = true,
                Reason = "The claim matches a policy exclusion."
            };
        }

        var approvedAmount =
            _claimAmountCalculator.CalculateApprovedAmount(
                context.Claim.ClaimedAmount,
                matchingCoverage.Limit,
                matchingCoverage.Deductible);

        var decision =
            approvedAmount == context.Claim.ClaimedAmount
                ? DecisionStatus.Approved
                : DecisionStatus.PartiallyApproved;

        return new AdjudicationResultDto
        {
            ClaimId = context.Claim.ClaimId,
            PolicyId = context.Policy.PolicyId,
            PolicyVersion = context.Policy.Version,
            Decision = decision,
            ClaimedAmount = context.Claim.ClaimedAmount,
            ApprovedAmount = approvedAmount,
            Deductible = matchingCoverage.Deductible,
            CoverageLimit = matchingCoverage.Limit,
            HasExclusion = false,
            Reason = "Claim is covered and the approved amount was calculated using the policy deductible and coverage limit."
        };
    }
}