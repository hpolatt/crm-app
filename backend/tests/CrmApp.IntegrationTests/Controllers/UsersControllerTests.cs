using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using CrmApp.Core.DTOs.Auth;
using CrmApp.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using CrmApp.Infrastructure.Data;

namespace CrmApp.IntegrationTests.Controllers;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBasicUsers_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/users/basic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBasicUsers_WithValidToken_ReturnsOkAndUserList()
    {
        // Arrange
        await SeedDefaultRole();
        var token = await GetAuthToken("basicuser@test.com", "Test", "User");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Seed some users
        await SeedUser("user1@test.com", "User", "One");
        await SeedUser("user2@test.com", "User", "Two");

        // Act
        var response = await _client.GetAsync("/api/users/basic");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("basicuser@test.com");
    }

    [Fact]
    public async Task GetAllUsers_AsRegularUser_ReturnsForbidden()
    {
        // Arrange
        await SeedDefaultRole();
        var token = await GetAuthToken("regularuser@test.com", "Regular", "User");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_AsAdmin_ReturnsOk()
    {
        // Arrange
        await SeedAdminRole();
        var adminToken = await GetAuthTokenWithRole("admin@test.com", "Admin", "User", "Admin");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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

    private async Task SeedAdminRole()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedDefaultRole();

        if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Description = "Admin role",
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

        if (await context.Users.AnyAsync(u => u.Email == email))
            return;

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

    private async Task<string> GetAuthToken(string email, string firstName, string lastName, string password = "Test123!@#")
    {
        await SeedUser(email, firstName, lastName, password);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        return result?.Token ?? throw new Exception("Failed to get token");
    }

    private async Task<string> GetAuthTokenWithRole(string email, string firstName, string lastName, string roleName, string password = "Test123!@#")
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = true,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assign role
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role != null)
        {
            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = role.Id
            };
            context.UserRoles.Add(userRole);
            await context.SaveChangesAsync();
        }

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });

        return result?.Token ?? throw new Exception("Failed to get token");
    }
}
