namespace DomainCopilot.Application.Interfaces;

public interface IDocumentStorage
{
    Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType);

    Task<Stream> OpenReadAsync(
        string storagePath);
}