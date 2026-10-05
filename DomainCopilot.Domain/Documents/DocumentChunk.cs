namespace DomainCopilot.Domain.Documents;

public class DocumentChunk
{
    public Guid DocumentChunkId { get; private set; }

    public Guid DocumentId { get; private set; }

    public Guid TenantId { get; private set; }

    public string PolicyNumber { get; private set; }

    public int PolicyVersion { get; private set; }

    public int PageNumber { get; private set; }

    public int ChunkIndex { get; private set; }

    public string Content { get; private set; }

    public string EmbeddingJson { get; private set; }

    public int CharacterCount { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DocumentChunk(
        Guid documentId,
        Guid tenantId,
        string policyNumber, 
        int policyVersion,
        int pageNumber,
        int chunkIndex,
        string content)
    {
        DocumentChunkId = Guid.NewGuid();

        DocumentId = documentId;

        TenantId = tenantId;
        
        PolicyNumber = policyNumber; 

        PolicyVersion = policyVersion;

        PageNumber = pageNumber;

        ChunkIndex = chunkIndex;

        Content = content;

        CharacterCount = content.Length;

        CreatedAt = DateTime.UtcNow;
    }
    public void SetEmbedding(string embeddingJson)
    {
        EmbeddingJson = embeddingJson;
    }
}