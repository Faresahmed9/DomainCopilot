using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.HasKey(c => c.ClaimId);

        builder.Property(c => c.PolicyNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ClaimNumber)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.IncidentDate)
            .IsRequired();

        builder.Property(c => c.SubmittedAt)
            .IsRequired();

        builder.Property(c => c.ClaimedAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.HasIndex(c => new
        {
            c.TenantId,
            c.ClaimNumber
        })
        .IsUnique();
    }
}