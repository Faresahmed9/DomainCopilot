using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Tests;

[Trait("Category", "Integration")]
public class TenantIsolationTests
{
    [Fact]
    public async Task GetActiveVersion_ShouldNotReturnAnotherTenantsPolicy()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new PolicyRepository(context);

        var tenantB =
            Guid.Parse("22222222-2222-2222-2222-222222222222");

        var policy =
            await repository.GetActiveVersionAsync(
                "POL-1001",
                new DateTime(2026, 6, 15),
                tenantB);

        Assert.Null(policy);
    }
}