using DomainCopilot.Domain;

namespace DomainCopilot.Application.Services;

public class ExclusionEvaluator
{
    public Exclusion? FindMatchingExclusion(
        Claim claim,
        IReadOnlyList<Exclusion> exclusions)
    {
        if (exclusions.Count == 0)
        {
            return null;
        }

        var matchingExclusion = exclusions.FirstOrDefault(
            exclusion =>
                claim.Description.Contains(
                    exclusion.ExclusionName,
                    StringComparison.OrdinalIgnoreCase));

        return matchingExclusion;
    }
}