
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DomainCopilot.Application.Auth;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly PasswordService _passwordService;

    public LoginUseCase(
        IUserRepository userRepository,
        IConfiguration configuration,
        PasswordService passwordService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordService = passwordService;
    }

    public async Task<LoginResponseDto?> ExecuteAsync(
        string username,
        string password,
        Guid tenantId)
    {
        var user =
            await _userRepository.GetByUsernameAsync(
                username,
                tenantId);

        if (user is null)
            return null;

        // Verify the password against the stored hash
        if (!_passwordService.VerifyPassword(
                user.Password,
                password))
        {
            return null;
        }

        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException(
                "JWT key is not configured.");

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.UserId.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()),

            new Claim(
                "TenantId",
                user.TenantId.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler()
                .WriteToken(token),

            Username = user.Username,

            Role = user.Role.ToString(),

            TenantId = user.TenantId
        };
    }
}

