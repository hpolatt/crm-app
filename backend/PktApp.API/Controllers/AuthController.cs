using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Email);

        // Simple mock authentication - accepts any credentials
        // TODO: Implement real authentication
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Email ve şifre gereklidir"
            });
        }

        var response = new ApiResponse<LoginResponse>
        {
            Success = true,
            Message = "Giriş başarılı",
            Data = new LoginResponse
            {
                Token = "mock-jwt-token-" + Guid.NewGuid().ToString(),
                User = new UserInfo
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Name = request.Email.Split('@')[0]
                }
            }
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Çıkış başarılı"
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserInfo User { get; set; } = null!;
}

public class UserInfo
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
