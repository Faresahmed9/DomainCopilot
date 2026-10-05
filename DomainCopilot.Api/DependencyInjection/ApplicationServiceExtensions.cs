//using DomainCopilot.Application.Services;
//using DomainCopilot.Application.UseCases;
//using Microsoft.Extensions.DependencyInjection;

//namespace DomainCopilot.Application.DependencyInjection;

//public static class ApplicationServiceExtensions
//{
//    public static IServiceCollection AddApplication(
//        this IServiceCollection services)
//    //{
//        // Use Cases
//        services.AddScoped<GetClaimContextUseCase>();
//        services.AddScoped<AdjudicateClaimUseCase>();

//        // Application Services
//        services.AddScoped<ClaimAmountCalculator>();
//        services.AddScoped<CoverageEvaluator>();
//        services.AddScoped<ExclusionEvaluator>();
//        services.AddScoped<AnomalyDetector>();

//        return services;
//    }
//}