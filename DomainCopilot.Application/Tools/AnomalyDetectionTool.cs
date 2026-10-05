using DomainCopilot.Application.Services;
using DomainCopilot.Domain;

namespace DomainCopilot.Application.Tools;

public class AnomalyDetectionTool
{
    private readonly AnomalyDetector _anomalyDetector;

    public AnomalyDetectionTool(
        AnomalyDetector anomalyDetector)
    {
        _anomalyDetector = anomalyDetector;
    }

    public IReadOnlyList<Anomaly> Execute(
        Claim claim,
        Coverage? coverage)
    {
        return _anomalyDetector.Detect(
            claim,
            coverage);
    }
}