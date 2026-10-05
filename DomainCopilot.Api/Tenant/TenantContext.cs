using System.Security.Claims;
using DomainCopilot.Application.Tenant;

namespace DomainCopilot.Api.Tenant;

public class TenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantContext(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid TenantId
    {
        get
        {
            var user =
                _httpContextAccessor.HttpContext?.User;

            var tenantClaim =
                user?.FindFirst("TenantId")?.Value;

            if (!Guid.TryParse(tenantClaim, out var tenantId))
            {
                throw new UnauthorizedAccessException(
                    "TenantId claim is missing or invalid.");
            }

            return tenantId;
        }
    }
}