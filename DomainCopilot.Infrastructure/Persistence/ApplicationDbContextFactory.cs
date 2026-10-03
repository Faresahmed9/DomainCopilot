using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DomainCopilot.Infrastructure.Persistence;

public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(
        string[] args)
    {
        var optionsBuilder =
            new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new ApplicationDbContext(
            optionsBuilder.Options);
    }
}