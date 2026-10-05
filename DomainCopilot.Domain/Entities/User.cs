using DomainCopilot.Domain.Enums;

namespace DomainCopilot.Domain;

public class User
{
    public Guid UserId { get; private set; }

    public Guid TenantId { get; private set; }

    public string Username { get; private set; }

    public string Password { get; private set; }

    public UserRole Role { get; private set; }

    public User(
        Guid userId,
        Guid tenantId,
        string username,
        string password,
        UserRole role)
    {
        UserId = userId;
        TenantId = tenantId;
        Username = username;
        Password = password;
        Role = role;
    }
}