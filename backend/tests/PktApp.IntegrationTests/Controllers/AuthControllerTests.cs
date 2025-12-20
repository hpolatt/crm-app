using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using PktApp.Infrastructure.Data;

namespace PktApp.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOkAndToken()
    {
        // Arrange
        await SeedDefaultRole();

        var registerRequest = new RegisterRequest
        {
            Email = "newuser@test.com",
            Password = "Test123!@#",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        
        var result = apiResponse.Data!;
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("newuser@test.com");
    }

    [Fact]
    public async Task Register_WithExistingEmail_ReturnsBadRequest()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedUser("existing@test.com", "Test", "User");

        var registerRequest = new RegisterRequest
        {
            Email = "existing@test.com",
            Password = "Test123!@#",
            FirstName = "Another",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndToken()
    {
        // Arrange
        await SeedDefaultRole();
        var password = "Test123!@#";
        await SeedUser("validuser@test.com", "Valid", "User", password);

        var loginRequest = new LoginRequest
        {
            Email = "validuser@test.com",
            Password = password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        
        var result = apiResponse.Data!;
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("validuser@test.com");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        await SeedDefaultRole();
        await SeedUser("user@test.com", "Test", "User", "CorrectPassword123!");

        var loginRequest = new LoginRequest
        {
            Email = "user@test.com",
            Password = "WrongPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonexistentEmail_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "SomePassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Helper methods
    private async Task SeedDefaultRole()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await context.Roles.AnyAsync(r => r.Name == "User"))
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "User",
                Description = "Default user role",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }
    }

    private async Task SeedUser(string email, string firstName, string lastName, string? password = null)
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password ?? "Test123!@#"),
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
    }
}
