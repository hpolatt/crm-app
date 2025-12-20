using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace PKT.API.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                       ?? User.FindFirst("sub")?.Value 
                       ?? User.FindFirst("userId")?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim))
            return null;
            
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    protected string? GetCurrentUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value 
            ?? User.FindFirst("email")?.Value;
    }

    protected string? GetCurrentUserName()
    {
        return User.FindFirst(ClaimTypes.Name)?.Value 
            ?? User.FindFirst("name")?.Value;
    }
}
