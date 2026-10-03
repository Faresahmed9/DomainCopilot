using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class ExclusionConfiguration : IEntityTypeConfiguration<Exclusion>
{
    public void Configure(EntityTypeBuilder<Exclusion> builder)
    {
        builder.HasKey(e => e.ExclusionId);

        builder.Property(e => e.ExclusionName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(2000);
    }
}