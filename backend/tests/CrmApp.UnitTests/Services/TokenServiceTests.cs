using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CrmApp.Domain.Entities;
using CrmApp.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CrmApp.UnitTests.Services;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration;

    public TokenServiceTests()
    {
        // Setup test configuration
        var configValues = new Dictionary<string, string>
        {
            {"JwtSettings:Secret", "TestSecretKeyForUnitTesting1234567890123456"},
            {"JwtSettings:Issuer", "CrmAppTest"},
            {"JwtSettings:Audience", "CrmAppTestUsers"},
            {"JwtSettings:ExpiryMinutes", "60"}
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        _tokenService = new TokenService(_configuration);
    }

    [Fact]
    public void GenerateAccessToken_WithValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // Verify token is valid JWT
        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
        
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Should().NotBeNull();
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User", "Admin" };

        // Act
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        // JWT uses short claim names by default
        jwtToken.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == "test@example.com");
        jwtToken.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == "John Doe");
        jwtToken.Claims.Should().Contain(c => c.Type == "FirstName" && c.Value == "John");
        jwtToken.Claims.Should().Contain(c => c.Type == "LastName" && c.Value == "Doe");
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "User");
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Admin");
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectIssuerAndAudience()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User" };

        // Act
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        jwtToken.Issuer.Should().Be("CrmAppTest");
        jwtToken.Audiences.Should().Contain("CrmAppTestUsers");
    }

    [Fact]
    public void GenerateAccessToken_HasCorrectExpiry()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User" };
        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        
        var expectedExpiry = beforeGeneration.AddMinutes(60);
        jwtToken.ValidTo.Should().BeCloseTo(expectedExpiry, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        // Act
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GenerateRefreshToken_GeneratesUniqueTokens()
    {
        // Act
        var token1 = _tokenService.GenerateRefreshToken();
        var token2 = _tokenService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User" };
        
        // Use the SAME TokenService instance that created the token
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Act - validate with same instance/config
        var result = _tokenService.ValidateToken(token);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userId);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var result = _tokenService.ValidateToken(invalidToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ReturnsNull()
    {
        // Act
        var result = _tokenService.ValidateToken(string.Empty);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithNullToken_ReturnsNull()
    {
        // Act
        var result = _tokenService.ValidateToken(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_WithDifferentSecret_ReturnsNull()
    {
        // Arrange - Create token with one secret
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var roles = new List<string> { "User" };
        var token = _tokenService.GenerateAccessToken(user, roles);

        // Create another TokenService with different secret for validation
        var differentConfigValues = new Dictionary<string, string>
        {
            {"JwtSettings:Secret", "DifferentSecretKeyForUnitTesting1234567890123456"},
            {"JwtSettings:Issuer", "CrmAppTest"},
            {"JwtSettings:Audience", "CrmAppTestUsers"},
            {"JwtSettings:ExpiryMinutes", "60"}
        };

        var differentConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(differentConfigValues!)
            .Build();

        var differentTokenService = new TokenService(differentConfig);

        // Act - Try to validate with different secret
        var result = differentTokenService.ValidateToken(token);

        // Assert - Should fail validation
        result.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMissingSecret_ThrowsInvalidOperationException()
    {
        // Arrange
        var configValues = new Dictionary<string, string>
        {
            {"JwtSettings:Issuer", "CrmAppTest"},
            {"JwtSettings:Audience", "CrmAppTestUsers"}
            // Secret is missing
        };

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues!)
            .Build();

        // Act & Assert
        var act = () => new TokenService(config);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT Secret not configured");
    }
}
