using System.Security.Claims;

namespace PktApp.API.Middleware;

public class RoleMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RoleMiddleware> _logger;

    public RoleMiddleware(RequestDelegate next, ILogger<RoleMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Token'dan veya header'dan role bilgisini al
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (!string.IsNullOrEmpty(token))
        {
            // Token'dan user bilgisini parse et (şimdilik basit bir yöntem)
            // Gerçek JWT kullanıldığında burası değişecek
            
            // Şimdilik custom header kullan
            var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(userRole))
            {
                var claims = new List<Claim>
                {
                    new Claim("role", userRole),
                    new Claim(ClaimTypes.Role, userRole)
                };
                
                var identity = new ClaimsIdentity(claims, "custom");
                context.User = new ClaimsPrincipal(identity);
                
                _logger.LogInformation("User role set to: {Role}", userRole);
            }
        }

        await _next(context);
    }
}
