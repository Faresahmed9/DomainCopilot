using DomainCopilot.Application.DTOs;

namespace DomainCopilot.Application.Interfaces;

public interface IDocumentTextExtractor
{
    Task<IReadOnlyList<DocumentPage>> ExtractPagesAsync(
        Stream fileStream,
        string contentType);
}

