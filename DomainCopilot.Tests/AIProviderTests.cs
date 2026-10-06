using DomainCopilot.Infrastructure.AI;

namespace DomainCopilot.Tests;

public class AIProviderTests
{
    [Fact]
    public async Task LocalAiProvider_ShouldReturnResponse()
    {
        var provider = new LocalAiProvider();

        var result = await provider.GenerateAsync(
            "Test system prompt",
            "Test user prompt");

        Assert.False(string.IsNullOrWhiteSpace(result));
        Assert.Contains("Local AI provider", result);
    }
}