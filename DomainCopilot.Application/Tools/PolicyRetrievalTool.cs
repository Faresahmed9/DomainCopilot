using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.UseCases;

namespace DomainCopilot.Application.Tools;

public class PolicyRetrievalTool
{
    private readonly RetrieveRelevantChunksUseCase _retrieveRelevantChunksUseCase;

    public PolicyRetrievalTool(
        RetrieveRelevantChunksUseCase retrieveRelevantChunksUseCase)
    {
        _retrieveRelevantChunksUseCase = retrieveRelevantChunksUseCase;
    }

    public async Task<IReadOnlyList<RetrievedChunkDto>> ExecuteAsync(
        Guid tenantId,
        string policyNumber,
        DateTime incidentDate,
        string query)
    {
        return await _retrieveRelevantChunksUseCase.ExecuteAsync(
            tenantId,
            policyNumber,
            incidentDate,
            query);
    }
}