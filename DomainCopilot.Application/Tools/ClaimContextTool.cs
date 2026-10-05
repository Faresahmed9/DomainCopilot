using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.UseCases;

namespace DomainCopilot.Application.Tools;

public class ClaimContextTool
{
    private readonly GetClaimContextUseCase _getClaimContextUseCase;

    public ClaimContextTool(
        GetClaimContextUseCase getClaimContextUseCase)
    {
        _getClaimContextUseCase = getClaimContextUseCase;
    }

    public async Task<ClaimContextDto?> ExecuteAsync(
        Guid claimId,
        Guid tenantId)
    {
        return await _getClaimContextUseCase.ExecuteAsync(
            claimId,
            tenantId);
    }
}