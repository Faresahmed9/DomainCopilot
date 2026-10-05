
using DomainCopilot.Application.Interfaces;
using DomainCopilot.Domain;

namespace DomainCopilot.Application.UseCases;

public class CreateDocumentUseCase
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IDocumentStorage _documentStorage;

    public CreateDocumentUseCase(
        IDocumentRepository documentRepository,
        IDocumentStorage documentStorage)
    {
        _documentRepository = documentRepository;
        _documentStorage = documentStorage;
    }

    public async Task<Document> ExecuteAsync(
        Guid tenantId,
        string policyNumber,
        int policyVersion,
        DateTime effectiveFrom,
        DateTime effectiveTo,
        string fileName,
        string contentType,
        Stream fileStream)
    {
        var storagePath = await _documentStorage.SaveAsync(
            fileStream,
            fileName,
            contentType);

        var document = new Document(
            tenantId,
            policyNumber,
            policyVersion,
            effectiveFrom,
            effectiveTo,
            fileName,
            contentType,
            storagePath);

        await _documentRepository.AddAsync(document);

        return document;
    }
}

