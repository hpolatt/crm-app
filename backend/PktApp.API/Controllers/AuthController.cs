using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IRepository<User> _userRepository;

    public AuthController(ILogger<AuthController> logger, IRepository<User> userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt - Email: '{Email}', Password length: {PasswordLength}", 
            request.Email ?? "(null)", 
            request.Password?.Length ?? 0);

        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            _logger.LogWarning("Login failed - Email or Password is empty");
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Kullanıcı adı ve şifre gereklidir"
            });
        }

        // Find user by username (email field is actually username)
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Username == request.Email && u.IsActive);

        if (user == null)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Kullanıcı adı veya şifre hatalı"
            });
        }

        // Verify password using BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify("huseyinpolat_" + user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") + request.Password, user.PasswordHash);
        
        if (!isPasswordValid)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Kullanıcı adı veya şifre hatalı"
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
                    Id = user.Id,
                    Email = user.Email,
                    Name = $"{user.FirstName} {user.LastName}",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString()
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
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
