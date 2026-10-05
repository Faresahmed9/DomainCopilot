using DomainCopilot.Application.DependencyInjection;
using DomainCopilot.Application.Tenant;
using DomainCopilot.Infrastructure.DependencyInjection;
using DomainCopilot.Api.Tenant;
using DomainCopilot.Api.Swagger;
using DomainCopilot.Api.Authentication;

namespace DomainCopilot.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<ITenantContext, TenantContext>();

        builder.Services.AddApplication();

        builder.Services.AddInfrastructure();

        // Authentication & Authorization
        builder.Services.AddJwtAuthentication(
            builder.Configuration);

        // Swagger / OpenAPI
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition(
                "Bearer",
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter JWT token"
                });

            options.AddSecurityRequirement(
                new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
                });

            options.OperationFilter<TenantHeaderOperationFilter>();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}