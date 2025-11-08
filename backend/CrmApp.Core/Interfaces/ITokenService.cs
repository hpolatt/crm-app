using CrmApp.Domain.Entities;

namespace CrmApp.Core.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
