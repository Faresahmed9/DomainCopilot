using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Infrastructure.Storage;

public class LocalDocumentStorage : IDocumentStorage
{
    private readonly string _storageRoot;

    public LocalDocumentStorage()
    {
        _storageRoot = Path.Combine(
            AppContext.BaseDirectory,
            "Storage");
    }

    public async Task<string> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType)
    {
        Directory.CreateDirectory(_storageRoot);

        var uniqueFileName =
            $"{Guid.NewGuid()}_{fileName}";

        var filePath = Path.Combine(
            _storageRoot,
            uniqueFileName);

        await using var outputStream =
            new FileStream(
                filePath,
                FileMode.Create);

        await fileStream.CopyToAsync(outputStream);

        return filePath;
    }

    public Task<Stream> OpenReadAsync(
        string storagePath)
    {
        Stream stream = new FileStream(
            storagePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        return Task.FromResult(stream);
    }
}