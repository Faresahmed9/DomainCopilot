using DomainCopilot.Domain;

namespace DomainCopilot.Application.Services;

public class CoverageEvaluator
{
    public Coverage? FindMatchingCoverage(
        Claim claim,
        IReadOnlyList<Coverage> coverages)
    {
        if (coverages.Count == 0)
        {
            return null;
        }

        var matchingCoverage = coverages.FirstOrDefault(
            coverage =>
                claim.Description.Contains(
                    coverage.CoverageName,
                    StringComparison.OrdinalIgnoreCase));

        return matchingCoverage;
    }
}