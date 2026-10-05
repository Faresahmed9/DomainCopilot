using DomainCopilot.Application.Interfaces;
using Google.GenAI;

namespace DomainCopilot.Infrastructure.AI;

public class GeminiAiProvider : IAiProvider
{
    private readonly Client _client;

    public GeminiAiProvider(string apiKey)
    {
        _client = new Client(apiKey: apiKey);
    }

    public async Task<string> GenerateAsync(
        string systemPrompt,
        string userPrompt)
    {
        var prompt = $"""
            System Instructions:
            {systemPrompt}

            User Request:
            {userPrompt}
            """;

        // Try primary model first.
        try
        {
            return await GenerateWithModelAsync(
                "gemini-3.8-flash",
                prompt);
        }
        catch (Google.GenAI.ClientError)
        {
            // Primary model may have exhausted its quota.
        }
        catch (Google.GenAI.ServerError)
        {
            // Primary model may be temporarily unavailable.
        }

        // Try fallback model.
        try
        {
            return await GenerateWithModelAsync(
                "gemini-3.5-flash-lite",
                prompt);
        }
        catch (Google.GenAI.ClientError)
        {
            throw new InvalidOperationException(
                "Gemini API quota is currently exhausted " +
                "for the configured models.");
        }
        catch (Google.GenAI.ServerError)
        {
            throw new InvalidOperationException(
                "Gemini service is temporarily unavailable. " +
                "Both configured models failed.");
        }
    }

    private async Task<string> GenerateWithModelAsync(
        string model,
        string prompt)
    {
        const int maxAttempts = 2;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var response =
                    await _client.Models.GenerateContentAsync(
                        model: model,
                        contents: prompt);

                var text = response.Candidates?
                    .FirstOrDefault()?
                    .Content?
                    .Parts?
                    .FirstOrDefault()?
                    .Text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    throw new InvalidOperationException(
                        $"Gemini model '{model}' did not return a valid response.");
                }

                return text;
            }
            catch (Google.GenAI.ServerError)
            {
                if (attempt == maxAttempts)
                    throw;

                await Task.Delay(
                    TimeSpan.FromSeconds(2 * attempt));
            }
        }

        throw new InvalidOperationException(
            $"Gemini model '{model}' failed unexpectedly.");
    }
}