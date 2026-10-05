using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.Agents;

public class AdjudicationDrafterAgent
{
    private readonly IAiProvider _aiProvider;

    public AdjudicationDrafterAgent(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    public async Task<string> DraftAsync(
        string claimDescription,
        decimal claimedAmount,
        string coverageAnalysis,
        string exclusionAnalysis,
        string policyEvidence)
    {
        var systemPrompt = """
            You are an Adjudication Drafter Agent for an insurance
            claims adjudication system.

            Your job is to draft a recommended claim decision
            based only on the provided claim and policy evidence.

            Consider:
            - Claim description
            - Claimed amount
            - Coverage analysis
            - Exclusion analysis
            - Policy evidence

            You may recommend:
            - Approved
            - Partially Approved
            - Rejected

            Do not calculate the approved amount.
            Do not perform arithmetic.
            Do not invent policy information.
            Do not invent coverage or exclusions.

            The final financial amount will be calculated
            deterministically by the application.

            Return a concise recommendation with:
            1. Recommended decision
            2. Reason
            3. Relevant coverage/exclusion findings
            """;

        var userPrompt = $"""
            Claim Description:
            {claimDescription}

            Claimed Amount:
            {claimedAmount}

            Coverage Analysis:
            {coverageAnalysis}

            Exclusion Analysis:
            {exclusionAnalysis}

            Policy Evidence:
            {policyEvidence}

            Draft the recommended adjudication decision.
            Do not calculate any approved amount.
            """;

        return await _aiProvider.GenerateAsync(
            systemPrompt,
            userPrompt);
    }
}

