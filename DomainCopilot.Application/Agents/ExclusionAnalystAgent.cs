using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.Agents;

public class ExclusionAnalystAgent
{
    private readonly IAiProvider _aiProvider;

    public ExclusionAnalystAgent(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    public async Task<string> AnalyzeAsync(
        string claimDescription,
        string policyEvidence)
    {
        var systemPrompt = """
            You are an Exclusion Analyst Agent for an insurance claims system.

            Your job is to identify whether the claim matches
            any exclusion in the provided policy evidence.

            Use only the provided policy evidence.
            Do not invent exclusions.
            Do not calculate approved amounts.

            If an exclusion matches, identify it and explain why.
            If no exclusion matches, clearly state that no matching
            exclusion was found.
            """;

        var userPrompt = $"""
            Claim Description:
            {claimDescription}

            Policy Evidence:
            {policyEvidence}

            Determine whether the claim matches a policy exclusion.
            """;

        return await _aiProvider.GenerateAsync(
            systemPrompt,
            userPrompt);
    }
}