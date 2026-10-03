using DomainCopilot.Domain;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<Policy> Policies { get; set; }

    public DbSet<Claim> Claims { get; set; }

    public DbSet<Coverage> Coverages { get; set; }

    public DbSet<Exclusion> Exclusions { get; set; }

    public DbSet<Anomaly> Anomalies { get; set; }

    public DbSet<AdjudicationDecision> AdjudicationDecisions { get; set; }

    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant → Policies
        modelBuilder.Entity<Policy>()
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(p => p.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Policy → Coverages
        modelBuilder.Entity<Coverage>()
            .HasOne<Policy>()
            .WithMany()
            .HasForeignKey(c => c.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Policy → Exclusions
        modelBuilder.Entity<Exclusion>()
            .HasOne<Policy>()
            .WithMany()
            .HasForeignKey(e => e.PolicyId)
            .OnDelete(DeleteBehavior.Cascade);

        // Policy → Claims
        modelBuilder.Entity<Claim>()
            .HasOne<Policy>()
            .WithMany()
            .HasForeignKey(c => c.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Claim → Anomalies
        modelBuilder.Entity<Anomaly>()
            .HasOne<Claim>()
            .WithMany()
            .HasForeignKey(a => a.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        // Claim → Adjudication Decisions
        modelBuilder.Entity<AdjudicationDecision>()
            .HasOne<Claim>()
            .WithMany()
            .HasForeignKey(d => d.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        // Adjudication Decision → Approval Request
        modelBuilder.Entity<ApprovalRequest>()
            .HasOne<AdjudicationDecision>()
            .WithOne()
            .HasForeignKey<ApprovalRequest>(
                a => a.AdjudicationDecisionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.ApplyConfigurationsFromAssembly(
        typeof(ApplicationDbContext).Assembly);
    }
}