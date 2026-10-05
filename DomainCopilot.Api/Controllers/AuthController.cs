using DomainCopilot.Application.Auth;
using DomainCopilot.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DomainCopilot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(
        LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromHeader(Name = "X-Tenant-Id")] Guid tenantId,
        [FromBody] LoginRequestDto request)
    {
        if (tenantId == Guid.Empty)
            return BadRequest("A valid X-Tenant-Id header is required.");

        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest("Username is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Password is required.");

        var result =
            await _loginUseCase.ExecuteAsync(
                request.Username,
                request.Password,
                tenantId);

        if (result is null)
            return Unauthorized("Invalid username or password.");

        return Ok(result);
    }
}