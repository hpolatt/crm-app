using PktApp.Domain.Entities;

namespace PktApp.Core.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
