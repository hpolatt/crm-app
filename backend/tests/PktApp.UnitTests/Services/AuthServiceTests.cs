using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;
using PktApp.Infrastructure.Services;
using PktApp.UnitTests.Helpers;

namespace PktApp.UnitTests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // Setup InMemory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        _tokenServiceMock = new Mock<ITokenService>();
        _authService = new AuthService(_context, _tokenServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        var role = TestDataFactory.CreateRole("User");
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        };
        user.UserRoles.Add(userRole);

        _context.Users.Add(user);
        _context.Roles.Add(role);
        _context.UserRoles.Add(userRole);
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "TestPassword123!"
        };

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("mock_access_token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("mock_refresh_token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("mock_access_token");
        result.RefreshToken.Should().Be("mock_refresh_token");
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be("test@example.com");
        result.User.Roles.Should().Contain("User");

        // Verify refresh token was saved
        var savedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == user.Id);
        savedRefreshToken.Should().NotBeNull();
        savedRefreshToken!.Token.Should().Be("mock_refresh_token");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "SomePassword123!"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword123!"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "inactive@example.com", isActive: false);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "inactive@example.com",
            Password = "TestPassword123!"
        };

        // Act
        Func<Task> act = async () => await _authService.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid email or password");
    }

    [Fact]
    public async Task RegisterAsync_WithNewUser_CreatesUserAndReturnsLoginResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "NewPassword123!",
            FirstName = "New",
            LastName = "User"
        };

        var defaultRole = TestDataFactory.CreateRole("User");
        _context.Roles.Add(defaultRole);
        await _context.SaveChangesAsync();

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("mock_access_token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("mock_refresh_token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("mock_access_token");
        result.User.Email.Should().Be("newuser@example.com");
        result.User.FirstName.Should().Be("New");
        result.User.LastName.Should().Be("User");

        // Verify user was created in database
        var createdUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
        createdUser.Should().NotBeNull();
        createdUser!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = TestDataFactory.CreateUser(email: "existing@example.com");
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "Password123!",
            FirstName = "Duplicate",
            LastName = "User"
        };

        // Act
        Func<Task> act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User with this email already exists");
    }

    [Fact]
    public async Task RegisterAsync_WithoutDefaultRole_ThrowsInvalidOperationException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            FirstName = "New",
            LastName = "User"
        };

        // Act - No "User" role in database
        Func<Task> act = async () => await _authService.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Default user role not found");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ReturnsNewTokens()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        var role = TestDataFactory.CreateRole("User");
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = role.Id,
            User = user,
            Role = role
        };
        user.UserRoles.Add(userRole);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "valid_refresh_token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            User = user
        };

        _context.Users.Add(user);
        _context.Roles.Add(role);
        _context.UserRoles.Add(userRole);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid_refresh_token"
        };

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<List<string>>()))
            .Returns("new_access_token");
        _tokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns("new_refresh_token");

        // Act
        var result = await _authService.RefreshTokenAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("new_access_token");
        result.RefreshToken.Should().Be("new_refresh_token");
        
        // Old token should be revoked
        var oldToken = await _context.RefreshTokens.FindAsync(refreshToken.Id);
        oldToken!.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new RefreshTokenRequest
        {
            RefreshToken = "invalid_token"
        };

        // Act
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithExpiredToken_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        var expiredToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "expired_token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired
            CreatedAt = DateTime.UtcNow.AddDays(-8),
            IsRevoked = false
        };

        _context.Users.Add(user);
        _context.RefreshTokens.Add(expiredToken);
        await _context.SaveChangesAsync();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "expired_token"
        };

        // Act
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid or expired refresh token");
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInactiveUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com", isActive: false);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "valid_token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            User = user
        };

        _context.Users.Add(user);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        var request = new RefreshTokenRequest
        {
            RefreshToken = "valid_token"
        };

        // Act
        Func<Task> act = async () => await _authService.RefreshTokenAsync(request);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not active");
    }

    [Fact]
    public async Task LogoutAsync_RevokesAllUserTokens()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        user.Id = userId;

        var token1 = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "token1",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        var token2 = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "token2",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.Users.Add(user);
        _context.RefreshTokens.AddRange(token1, token2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LogoutAsync(userId);

        // Assert
        result.Should().BeTrue();
        
        var tokens = await _context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
        tokens.Should().AllSatisfy(t => t.IsRevoked.Should().BeTrue());
    }

    [Fact]
    public async Task ActivateUserAsync_ActivatesUser()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com", isActive: false);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.ActivateUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        
        var activatedUser = await _context.Users.FindAsync(user.Id);
        activatedUser!.IsActive.Should().BeTrue();
        activatedUser.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task ActivateUserAsync_WithInvalidUserId_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _authService.ActivateUserAsync(invalidUserId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task DeactivateUserAsync_DeactivatesUserAndRevokesTokens()
    {
        // Arrange
        var user = TestDataFactory.CreateUser(email: "test@example.com");
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = "active_token",
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.Users.Add(user);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.DeactivateUserAsync(user.Id);

        // Assert
        result.Should().BeTrue();
        
        var deactivatedUser = await _context.Users.FindAsync(user.Id);
        deactivatedUser!.IsActive.Should().BeFalse();
        deactivatedUser.UpdatedAt.Should().NotBeNull();
        
        var revokedToken = await _context.RefreshTokens.FindAsync(refreshToken.Id);
        revokedToken!.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateUserAsync_WithInvalidUserId_ThrowsInvalidOperationException()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await _authService.DeactivateUserAsync(invalidUserId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("User not found");
    }

    public void Dispose()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }
}
