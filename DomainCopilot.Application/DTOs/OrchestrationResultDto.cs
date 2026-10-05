using DomainCopilot.Domain;
using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.DTOs;

public class OrchestrationResultDto
{
    public Guid ClaimId { get; set; }

    public Guid PolicyId { get; set; }

    public int PolicyVersion { get; set; }

    public DecisionStatus Decision { get; set; }

    public decimal ClaimedAmount { get; set; }

    public decimal ApprovedAmount { get; set; }

    public decimal Deductible { get; set; }

    public decimal CoverageLimit { get; set; }

    public bool HasExclusion { get; set; }

    public string CoverageAnalysis { get; set; } = string.Empty;

    public string ExclusionAnalysis { get; set; } = string.Empty;

    public string AdjudicationDraft { get; set; } = string.Empty;

    public IReadOnlyList<RetrievedChunkDto> RetrievedChunks { get; set; }
        = new List<RetrievedChunkDto>();

    public IReadOnlyList<Anomaly> Anomalies { get; set; }
        = new List<Anomaly>();

    // Human Approval Gate
    public Guid ApprovalRequestId { get; set; }
}