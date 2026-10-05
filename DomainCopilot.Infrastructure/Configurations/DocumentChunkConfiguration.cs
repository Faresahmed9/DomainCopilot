using DomainCopilot.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class DocumentChunkConfiguration
    : IEntityTypeConfiguration<DocumentChunk>
{
    public void Configure(EntityTypeBuilder<DocumentChunk> builder)
    {
        builder.ToTable("DocumentChunks");

        builder.HasKey(x => x.DocumentChunkId);

        builder.Property(x => x.PolicyNumber)
           .IsRequired()
           .HasMaxLength(100);

        builder.Property(x => x.PolicyVersion)
            .IsRequired();

        builder.Property(x => x.PageNumber)
       .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(x => x.EmbeddingJson)
        .IsRequired(false);

        builder.Property(x => x.ChunkIndex)
            .IsRequired();

        builder.Property(x => x.CharacterCount)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.DocumentId,
            x.ChunkIndex
        });
    }
}