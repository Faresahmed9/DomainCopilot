using DomainCopilot.Domain;

namespace DomainCopilot.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(
        string username,
        Guid tenantId);
}