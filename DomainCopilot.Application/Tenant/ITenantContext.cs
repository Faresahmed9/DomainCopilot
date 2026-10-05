namespace DomainCopilot.Application.Tenant;

public interface ITenantContext
{
    Guid TenantId { get; }
}