using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.DealStages;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class DealStagesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public DealStagesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/dealstages");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_WithValidToken_ReturnsOkAndStages()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dealstages");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DealStageDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAll_WithActiveFilter_ReturnsFilteredStages()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/dealstages?isActive=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<DealStageDto>>>();
        result!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetById_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var testId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/dealstages/{testId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var testId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/dealstages/{testId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateDealStageDto
        {
            Name = "Test Stage",
            Order = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/dealstages", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedStage()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateDealStageDto
        {
            Name = "New Stage",
            Order = 10,
            Description = "Test stage",
            Color = "#FF5733",
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/dealstages", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DealStageDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("New Stage");
    }

    [Fact]
    public async Task Update_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var testId = Guid.NewGuid();
        var updateDto = new UpdateDealStageDto
        {
            Name = "Updated Stage",
            Order = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/dealstages/{testId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Update_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var testId = Guid.NewGuid();
        var updateDto = new UpdateDealStageDto
        {
            Name = "Updated Stage",
            Order = 2
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/dealstages/{testId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var testId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/dealstages/{testId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var testId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/dealstages/{testId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"dealstagestest_{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User",
            Phone = "+1234567890"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        return result?.Data?.Token ?? string.Empty;
    }
}
