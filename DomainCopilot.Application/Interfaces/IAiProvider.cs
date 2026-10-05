namespace DomainCopilot.Application.Interfaces;

public interface IAiProvider
{
    Task<string> GenerateAsync(
        string systemPrompt,
        string userPrompt);
}