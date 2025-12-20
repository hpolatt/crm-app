using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using PktApp.Core.Interfaces;
using PktApp.Core.DTOs.Auth;
using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;

namespace PktApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthService(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    // Hardcoded users for demo purposes
    private readonly Dictionary<string, (string password, string firstName, string lastName, List<string> roles)> _hardcodedUsers = new()
    {
        ["anonymous"] = ("anonymous123", "Anonymous", "User", new List<string> { "User" }),
        ["admin"] = ("admin123", "Admin", "User", new List<string> { "Admin" }),
        ["ekinkirmizitoprak"] = ("ekin123", "Ekin", "Kırmızıtoprak", new List<string> { "User" })
    };

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Check hardcoded users first
        if (_hardcodedUsers.TryGetValue(request.Email.ToLower(), out var hardcodedUser))
        {
            if (request.Password == hardcodedUser.password)
            {
                var userId = Guid.NewGuid();
                var accessToken = _tokenService.GenerateAccessToken(new User 
                { 
                    Id = userId, 
                    Email = request.Email,
                    FirstName = hardcodedUser.firstName,
                    LastName = hardcodedUser.lastName 
                }, hardcodedUser.roles);
                var refreshToken = _tokenService.GenerateRefreshToken();

                return new LoginResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken,
                    User = new UserDto
                    {
                        Id = userId,
                        Email = request.Email,
                        FirstName = hardcodedUser.firstName,
                        LastName = hardcodedUser.lastName,
                        Roles = hardcodedUser.roles
                    }
                };
            }
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Check database users
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted, cancellationToken);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Get default "User" role
        var userRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);

        if (userRole == null)
        {
            throw new InvalidOperationException("Default user role not found");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Assign role
        var userRoleEntity = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = userRole.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserRoles.Add(userRoleEntity);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var roles = new List<string> { userRole.Name };
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };
    }

    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsRevoked, cancellationToken);

        if (refreshToken == null || refreshToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = refreshToken.User;
        if (!user.IsActive || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("User is not active");
        }

        // Revoke old refresh token
        refreshToken.IsRevoked = true;

        // Generate new tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Token = accessToken,
            RefreshToken = newRefreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            }
        };
    }

    public async Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ActivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.IsActive = true;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        
        // Revoke all refresh tokens
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
