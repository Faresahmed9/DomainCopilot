using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.HasKey(p => p.PolicyId);

        builder.Property(p => p.PolicyNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.PolicyName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Version)
            .IsRequired();

        builder.Property(p => p.EffectiveFrom)
            .IsRequired();

        builder.Property(p => p.EffectiveTo)
            .IsRequired();
    }
}