using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;

namespace PktApp.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected string? GetUserRole()
    {
        var roleClaim = User?.Claims?.FirstOrDefault(c => c.Type == "role" || c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        return roleClaim?.Value;
    }

    protected bool IsAdmin()
    {
        var role = GetUserRole();
        return role == "Admin";
    }

    protected bool IsForeman()
    {
        var role = GetUserRole();
        return role == "Foreman";
    }

    protected bool IsPowerUser()
    {
        var role = GetUserRole();
        return role == "PowerUser";
    }

    protected ActionResult<ApiResponse<T>> ForbiddenResponse<T>(string message = "Bu işlem için yetkiniz yok")
    {
        return StatusCode(403, ApiResponse<T>.ErrorResponse(message));
    }
}
