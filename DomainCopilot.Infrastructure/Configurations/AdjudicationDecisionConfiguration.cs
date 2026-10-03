using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class AdjudicationDecisionConfiguration
    : IEntityTypeConfiguration<AdjudicationDecision>
{
    public void Configure(
        EntityTypeBuilder<AdjudicationDecision> builder)
    {
        builder.HasKey(d => d.AdjudicationDecisionId);

        builder.Property(d => d.Decision)
            .IsRequired();

        builder.Property(d => d.ApprovedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(d => d.Reason)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(d => d.CreatedAt)
            .IsRequired();
    }
}