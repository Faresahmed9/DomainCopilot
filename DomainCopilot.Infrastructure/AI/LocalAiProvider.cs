using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Infrastructure.AI;

public class LocalAiProvider : IAiProvider
{
    public Task<string> GenerateAsync(
        string systemPrompt,
        string userPrompt)
    {
        var response =
            "Local AI provider response. " +
            "This provider is used as an alternative " +
            "implementation of IAiProvider.";

        return Task.FromResult(response);
    }
}