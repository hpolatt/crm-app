using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Leads;
using PktApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class LeadsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public LeadsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLead_WithValidData_ReturnsCreatedLead()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateLeadDto
        {
            Title = "New Business Opportunity",
            Description = "Potential client interested in our services",
            Source = "Website",
            Status = "new",
            Value = 50000,
            Probability = 30,
            ExpectedCloseDate = DateTime.UtcNow.AddMonths(3)
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/leads", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("New Business Opportunity");
        result.Data.Status.Should().Be("new");
        result.Data.Value.Should().Be(50000);
        result.Data.Probability.Should().Be(30);
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateLead_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateLeadDto
        {
            Title = "Test Lead"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/leads", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLeadById_WithExistingId_ReturnsLead()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a lead
        var createDto = new CreateLeadDto
        {
            Title = "Get Test Lead",
            Status = "qualified"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/leads", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        var leadId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/leads/{leadId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(leadId);
        result.Data.Title.Should().Be("Get Test Lead");
        result.Data.Status.Should().Be("qualified");
    }

    [Fact]
    public async Task GetLeadById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/leads/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllLeads_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few leads
        for (int i = 1; i <= 3; i++)
        {
            var createDto = new CreateLeadDto
            {
                Title = $"Lead {i}",
                Status = "new"
            };
            await _client.PostAsJsonAsync("/api/leads", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/leads?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<LeadDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateLead_WithValidData_ReturnsUpdatedLead()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a lead first
        var createDto = new CreateLeadDto
        {
            Title = "Original Lead",
            Status = "new",
            Probability = 20
        };
        var createResponse = await _client.PostAsJsonAsync("/api/leads", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        var leadId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateLeadDto
        {
            Title = "Updated Lead",
            Status = "qualified",
            Probability = 50,
            Value = 75000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/leads/{leadId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Updated Lead");
        result.Data.Status.Should().Be("qualified");
        result.Data.Probability.Should().Be(50);
        result.Data.Value.Should().Be(75000);
    }

    [Fact]
    public async Task UpdateLead_StatusChange_UpdatesSuccessfully()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a lead
        var createDto = new CreateLeadDto
        {
            Title = "Status Test Lead",
            Status = "new"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/leads", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        var leadId = createResult!.Data!.Id;

        // Update status from new -> qualified -> won
        var updateDto1 = new UpdateLeadDto { Status = "qualified" };
        var response1 = await _client.PutAsJsonAsync($"/api/leads/{leadId}", updateDto1);
        var result1 = await response1.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        result1!.Data!.Status.Should().Be("qualified");

        var updateDto2 = new UpdateLeadDto { Status = "won" };
        var response2 = await _client.PutAsJsonAsync($"/api/leads/{leadId}", updateDto2);
        
        // Assert
        response2.StatusCode.Should().Be(HttpStatusCode.OK);
        var result2 = await response2.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        result2!.Data!.Status.Should().Be("won");
    }

    [Fact]
    public async Task UpdateLead_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateLeadDto
        {
            Title = "Updated Lead"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/leads/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLead_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a lead first
        var createDto = new CreateLeadDto
        {
            Title = "Lead To Delete"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/leads", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<LeadDto>>();
        var leadId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/leads/{leadId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify lead is deleted
        var getResponse = await _client.GetAsync($"/api/leads/{leadId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteLead_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/leads/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"leadtest_{Guid.NewGuid()}@test.com",
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
