using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PKT.Application.Interfaces;
using PKT.Application.DTOs;
using PKT.Application.DTOs.Auth;

namespace PKT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Login failed for {Email}", request.Email);
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", request.Email);
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("An error occurred during login"));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Registration successful"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Registration failed for {Email}", request.Email);
            return BadRequest(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("An error occurred during registration"));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result, "Token refreshed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Refresh token failed");
            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, ApiResponse<LoginResponse>.ErrorResponse("An error occurred during token refresh"));
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _authService.LogoutAsync(userId.Value);
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred during logout"));
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
    }
}
