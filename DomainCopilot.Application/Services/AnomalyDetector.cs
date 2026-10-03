using DomainCopilot.Domain;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.Services;

public class AnomalyDetector
{
    public IReadOnlyList<Anomaly> Detect(
        Claim claim,
        Coverage? coverage)
    {
        var anomalies = new List<Anomaly>();

        if (claim.ClaimedAmount <= 0)
        {
            anomalies.Add(
                new Anomaly(
                    claim.ClaimId,
                    "InvalidAmount",
                    "Claimed amount must be greater than zero.",
                    AnomalySeverity.High));
        }

        if (claim.IncidentDate > claim.SubmittedAt)
        {
            anomalies.Add(
                new Anomaly(
                    claim.ClaimId,
                    "InvalidDate",
                    "Incident date cannot be after claim submission date.",
                    AnomalySeverity.High));
        }

        if (coverage is not null &&
            claim.ClaimedAmount > coverage.Limit)
        {
            anomalies.Add(
                new Anomaly(
                    claim.ClaimId,
                    "AmountExceedsLimit",
                    "Claimed amount exceeds the coverage limit.",
                    AnomalySeverity.Medium));
        }

        return anomalies;
    }
}