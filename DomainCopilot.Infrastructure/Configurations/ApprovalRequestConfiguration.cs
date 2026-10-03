using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DomainCopilot.Infrastructure.Persistence.Configurations;

public class ApprovalRequestConfiguration
    : IEntityTypeConfiguration<ApprovalRequest>
{
    public void Configure(
        EntityTypeBuilder<ApprovalRequest> builder)
    {
        builder.HasKey(a => a.ApprovalRequestId);

        builder.Property(a => a.Status)
            .IsRequired();

        builder.Property(a => a.ReviewerId)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.ReviewerComment)
            .HasMaxLength(2000);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.ReviewedAt)
            .IsRequired(false);
    }
}