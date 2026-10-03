using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class AnomalyConfiguration : IEntityTypeConfiguration<Anomaly>
{
    public void Configure(EntityTypeBuilder<Anomaly>
        builder)
    {
        builder.HasKey(a => a.AnomalyId);

        builder.Property(a => a.AnomalyType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(a => a.Severity)
            .IsRequired();

        builder.Property(a => a.DetectedAt)
            .IsRequired();
    }
}