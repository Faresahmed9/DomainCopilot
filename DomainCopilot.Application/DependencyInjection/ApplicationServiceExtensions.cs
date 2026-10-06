using DomainCopilot.Application.Agents;
using DomainCopilot.Application.Auth;
using DomainCopilot.Application.Orchestration;
using DomainCopilot.Application.Services;
using DomainCopilot.Application.Tools;
using DomainCopilot.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace DomainCopilot.Application.DependencyInjection;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // Use Cases
        services.AddScoped<GetClaimContextUseCase>();
        services.AddScoped<AdjudicateClaimUseCase>();
        services.AddScoped<ApproveAdjudicationUseCase>();

        services.AddScoped<CreateDocumentUseCase>();
        services.AddScoped<ProcessDocumentUseCase>();
        services.AddScoped<GenerateEmbeddingUseCase>();
        services.AddScoped<RetrieveRelevantChunksUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<PasswordService>();
        services.AddScoped<GetDashboardSummaryUseCase>();

        // Agents
        services.AddScoped<CoverageMatcherAgent>();
        services.AddScoped<ExclusionAnalystAgent>();
        services.AddScoped<AdjudicationDrafterAgent>();

        // Orchestration
        services.AddScoped<ClaimAdjudicationOrchestrator>();

        // Domain Services
        services.AddScoped<ClaimAmountCalculator>();
        services.AddScoped<CoverageEvaluator>();
        services.AddScoped<ExclusionEvaluator>();
        services.AddScoped<AnomalyDetector>();
        services.AddScoped<CosineSimilarityCalculator>();
        services.AddScoped<KeywordSearchService>();

        // Tools
        services.AddScoped<PolicyRetrievalTool>();
        services.AddScoped<ClaimContextTool>();
        services.AddScoped<ClaimAmountCalculatorTool>();
        services.AddScoped<AnomalyDetectionTool>();

        return services;
    }
}