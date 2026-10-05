
namespace DomainCopilot.Domain;

public class Document
{
    public Guid DocumentId { get; private set; }

    public Guid TenantId { get; private set; }

    public string PolicyNumber { get; private set; }

    public int PolicyVersion { get; private set; }

    public DateTime EffectiveFrom { get; private set; }

    public DateTime EffectiveTo { get; private set; }

    public string FileName { get; private set; }

    public string ContentType { get; private set; }

    public string StoragePath { get; private set; }

    public string Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public Document(
        Guid tenantId,
        string policyNumber,
        int policyVersion,
        DateTime effectiveFrom,
        DateTime effectiveTo,
        string fileName,
        string contentType,
        string storagePath)
    {
        DocumentId = Guid.NewGuid();
        TenantId = tenantId;
        PolicyNumber = policyNumber;
        PolicyVersion = policyVersion;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        FileName = fileName;
        ContentType = contentType;
        StoragePath = storagePath;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }
}

