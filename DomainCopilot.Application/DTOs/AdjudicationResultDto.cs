using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Application.DTOs;

public class AdjudicationResultDto
{
    public Guid ClaimId { get; set; }

    public Guid PolicyId { get; set; }

    public int PolicyVersion { get; set; }

    public DecisionStatus Decision { get; set; }

    public decimal ClaimedAmount { get; set; }

    public decimal ApprovedAmount { get; set; }

    public decimal Deductible { get; set; }

    public decimal CoverageLimit { get; set; }

    public string Reason { get; set; }

    public bool HasExclusion { get; set; }
}