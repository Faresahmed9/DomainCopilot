namespace DomainCopilot.Application.DTOs;

public class RetrievedChunkDto
{
    public Guid DocumentChunkId { get; set; }

    public string PolicyNumber { get; set; }

    public int PolicyVersion { get; set; }

    public int PageNumber { get; set; }

    public string Content { get; set; }

    public double Score { get; set; }
}