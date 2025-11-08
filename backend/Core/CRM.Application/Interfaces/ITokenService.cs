using CRM.Domain.Entities;

namespace CRM.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
