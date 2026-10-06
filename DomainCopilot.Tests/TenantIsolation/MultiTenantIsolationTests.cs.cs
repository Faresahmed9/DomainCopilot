using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DomainCopilot.Tests;

public class MultiTenantIsolationTests
{
    private static readonly Guid TenantA =
        Guid.Parse("A071EDBD-A4C8-4C53-81BB-9CEF1C72ECB2");

    private static readonly Guid TenantB =
        Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task TenantA_ShouldSeeOnlyTenantAPolicy()
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new DocumentChunkRepository(context);

        var results =
            await repository.SearchAsync(
                TenantA,
                "POL-1001",
                new DateTime(2026, 6, 15));

        Assert.NotEmpty(results);

        Assert.All(
            results,
            result =>
            {
                Assert.Equal(TenantA, result.TenantId);
                Assert.Equal("POL-1001", result.PolicyNumber);
            });
    }

    [Fact]
    public async Task TenantA_ShouldNotSeeTenantBPolicy()
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new DocumentChunkRepository(context);

        var results =
            await repository.SearchAsync(
                TenantA,
                "POL-1004",
                new DateTime(2026, 6, 15));

        Assert.Empty(results);
    }

    [Fact]
    public async Task TenantB_ShouldNotSeeTenantAPolicy()
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new DocumentChunkRepository(context);

        var results =
            await repository.SearchAsync(
                TenantB,
                "POL-1001",
                new DateTime(2026, 6, 15));

        Assert.Empty(results);
    }

    [Fact]
    public async Task TenantB_ShouldSeeOnlyTenantBPolicy()
    {
        var options =
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new DocumentChunkRepository(context);

        var results =
            await repository.SearchAsync(
                TenantB,
                "POL-1004",
                new DateTime(2026, 6, 15));

        Assert.NotEmpty(results);

        Assert.All(
            results,
            result =>
            {
                Assert.Equal(TenantB, result.TenantId);
                Assert.Equal("POL-1004", result.PolicyNumber);
            });
    }
}