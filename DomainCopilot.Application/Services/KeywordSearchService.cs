namespace DomainCopilot.Application.Services;

public class KeywordSearchService
{
    public double CalculateScore(string query, string content)
    {
        if (string.IsNullOrWhiteSpace(query) ||
            string.IsNullOrWhiteSpace(content))
        {
            return 0;
        }

        var queryWords = query
            .ToLowerInvariant()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        var contentWords = content
            .ToLowerInvariant()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        if (queryWords.Length == 0)
            return 0;

        var matchedWords = queryWords
            .Distinct()
            .Count(word => contentWords.Contains(word));

        return (double)matchedWords / queryWords.Distinct().Count();
    }
}