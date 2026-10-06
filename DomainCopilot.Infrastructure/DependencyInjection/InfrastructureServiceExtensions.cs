using DomainCopilot.Application.Interfaces;
using DomainCopilot.Infrastructure.AI;
using DomainCopilot.Infrastructure.DocumentProcessing;
using DomainCopilot.Infrastructure.Embeddings;
using DomainCopilot.Infrastructure.Persistence;
using DomainCopilot.Infrastructure.Repositories;
using DomainCopilot.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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

        // Adjudication and Approval repositories
        services.AddScoped<IAdjudicationRepository, AdjudicationRepository>();
        services.AddScoped<IApprovalRepository, ApprovalRepository>();

        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentStorage, LocalDocumentStorage>();
        services.AddScoped<IDocumentTextExtractor, PdfTextExtractor>();
        services.AddScoped<IChunkingService, ChunkingService>();
        services.AddScoped<IDocumentChunkRepository, DocumentChunkRepository>();
        services.AddScoped<ITextCleaner, TextCleaner>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IEmbeddingService>(sp =>
        {
            var configuration =
                sp.GetRequiredService<IConfiguration>();

            var apiKey = configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Gemini API key is not configured.");

            return new GeminiEmbeddingService(apiKey);
        });

        services.AddScoped<IAiProvider>(sp =>
        {
            var configuration =
                sp.GetRequiredService<IConfiguration>();

            var provider =
                configuration["AI:Provider"] ?? "Gemini";

            if (provider.Equals(
                "Local",
                StringComparison.OrdinalIgnoreCase))
            {
                return new LocalAiProvider();
            }

            var apiKey = configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException(
                    "Gemini API key is not configured.");

            return new GeminiAiProvider(apiKey);
        });

        return services;
    }


}
