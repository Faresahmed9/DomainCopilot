using DomainCopilot.Application.Interfaces;
using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DomainCopilot.Infrastructure.DependencyInjection;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                "Server=DESKTOP-42I26G0;Database=DomainCopilotDb;Trusted_Connection=True;TrustServerCertificate=True;"));

        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<ICoverageRepository, CoverageRepository>();
        services.AddScoped<IExclusionRepository, ExclusionRepository>();
        services.AddScoped<IAnomalyRepository, AnomalyRepository>();
        services.AddScoped<IAdjudicationRepository, AdjudicationRepository>();
        services.AddScoped<IApprovalRepository, ApprovalRepository>();

        return services;
    }
}