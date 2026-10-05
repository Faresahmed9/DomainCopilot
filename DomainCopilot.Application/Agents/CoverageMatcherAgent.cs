using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Application.Agents;

public class CoverageMatcherAgent
{
    private readonly IAiProvider _aiProvider;

    public CoverageMatcherAgent(IAiProvider aiProvider)
    {
        _aiProvider = aiProvider;
    }

    public async Task<string> MatchAsync(
        string claimDescription,
        string policyEvidence)
    {
        var systemPrompt = """
            You are a Coverage Matcher Agent for an insurance claims system.

            Your job is to identify which policy coverage best matches
            the claim description.

            Use only the provided policy evidence.
            Do not invent policy information.
            Do not calculate approved amounts.
            Return a concise explanation of the matching coverage.
            """;

        var userPrompt = $"""
            Claim Description:
            {claimDescription}

            Policy Evidence:
            {policyEvidence}

            Identify the best matching coverage and explain why.
            """;

        return await _aiProvider.GenerateAsync(
            systemPrompt,
            userPrompt);
    }
}