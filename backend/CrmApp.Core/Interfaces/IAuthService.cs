using CrmApp.Core.DTOs.Auth;

namespace CrmApp.Core.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
