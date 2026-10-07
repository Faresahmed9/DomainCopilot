using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DomainCopilot.Tests;
[Trait("Category", "Integration")]
public class PolicyVersionTests
{
    [Fact]
    public async Task GetActiveVersion_ShouldReturnVersionOneFor2025()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(
                "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        await using var context =
            new ApplicationDbContext(options);

        var repository =
            new PolicyRepository(context);

        var tenantId =
            Guid.Parse("A071EDBD-A4C8-4C53-81BB-9CEF1C72ECB2");

        var policy =
            await repository.GetActiveVersionAsync(
                "POL-1001",
                new DateTime(2025, 6, 15),
                tenantId);

        Assert.NotNull(policy);
        Assert.Equal(1, policy.Version);
    }
}