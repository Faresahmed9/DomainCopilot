using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainCopilot.Application.UseCases
{
    public class GetClaimContextUseCase
    {
        private readonly IClaimRepository _claimRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly ICoverageRepository _coverageRepository;
        private readonly IExclusionRepository _exclusionRepository;

        public GetClaimContextUseCase(
            IClaimRepository claimRepository,
            IPolicyRepository policyRepository,
            ICoverageRepository coverageRepository,
            IExclusionRepository exclusionRepository)
        {
            _claimRepository = claimRepository;
            _policyRepository = policyRepository;
            _coverageRepository = coverageRepository;
            _exclusionRepository = exclusionRepository;
        }

        public async Task<ClaimContextDto?> ExecuteAsync(Guid claimId)
        {
            // 1. Get the claim
            var claim = await _claimRepository.GetByIdAsync(claimId);

            if (claim is null)
            {
                return null;
            }

            // 2. Get the policy version active on incident date      // Wrong Policy Version
            var policy = await _policyRepository.GetActiveVersionAsync(
                claim.PolicyNumber,
                claim.IncidentDate);

            if (policy is null)
            {
                return null;
            }

            // 3. Get policy coverages
            var coverages = await _coverageRepository.GetByPolicyIdAsync(
                policy.PolicyId);

            // 4. Get policy exclusions
            var exclusions = await _exclusionRepository.GetByPolicyIdAsync(
                policy.PolicyId);

            // 5. Build the complete claim context
            return new ClaimContextDto
            {
                Claim = claim,
                Policy = policy,
                Coverages = coverages,
                Exclusions = exclusions
            };
        }
    }
}
