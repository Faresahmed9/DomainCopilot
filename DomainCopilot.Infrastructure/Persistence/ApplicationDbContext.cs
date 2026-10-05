using DomainCopilot.Domain;
using DomainCopilot.Domain.Documents;
using DomainCopilot.Domain.Enums;
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

    public DbSet<Document> Documents { get; set; }

    public DbSet<DocumentChunk> DocumentChunks { get; set; }

    public DbSet<User> Users { get; set; }


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


        modelBuilder.Entity<User>()
    .HasKey(x => x.UserId);

        modelBuilder.Entity<User>()
            .Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<User>()
            .Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<User>()
            .Property(x => x.Role)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasIndex(x => new
            {
                x.TenantId,
                x.Username
            })
            .IsUnique();



        modelBuilder.Entity<User>().HasData(
    new User(
        Guid.Parse("A1111111-1111-1111-1111-111111111111"),
        Guid.Parse("A071EDBD-A4C8-4C53-81BB-9CEF1C72ECB2"),
        "admin.a",
        "Admin123!",
        UserRole.Admin),

    new User(
        Guid.Parse("A2222222-2222-2222-2222-222222222222"),
        Guid.Parse("A071EDBD-A4C8-4C53-81BB-9CEF1C72ECB2"),
        "adjuster.a",
        "Adjuster123!",
        UserRole.Adjuster),

    new User(
        Guid.Parse("B1111111-1111-1111-1111-111111111111"),
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        "admin.b",
        "Admin123!",
        UserRole.Admin),

    new User(
        Guid.Parse("B2222222-2222-2222-2222-222222222222"),
        Guid.Parse("22222222-2222-2222-2222-222222222222"),
        "adjuster.b",
        "Adjuster123!",
        UserRole.Adjuster)
);
    }




}