using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class CoverageConfiguration : IEntityTypeConfiguration<Coverage>
{
    public void Configure(EntityTypeBuilder<Coverage> builder)
    {
        builder.HasKey(c => c.CoverageId);

        builder.Property(c => c.CoverageName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.Limit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Deductible)
            .HasPrecision(18, 2)
            .IsRequired();
    }
}