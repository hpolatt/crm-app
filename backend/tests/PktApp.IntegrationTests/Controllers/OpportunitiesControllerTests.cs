using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Opportunities;
using PktApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class OpportunitiesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OpportunitiesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOpportunity_WithValidData_ReturnsCreatedOpportunity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateOpportunityDto
        {
            Title = "New Sales Opportunity",
            Description = "Large enterprise deal",
            Stage = "prospecting",
            Value = 100000,
            Probability = 25,
            ExpectedCloseDate = DateTime.UtcNow.AddMonths(6),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/opportunities", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("New Sales Opportunity");
        result.Data.Stage.Should().Be("prospecting");
        result.Data.Value.Should().Be(100000);
        result.Data.Probability.Should().Be(25);
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateOpportunity_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "prospecting",
            Value = 50000
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/opportunities", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOpportunityById_WithExistingId_ReturnsOpportunity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an opportunity
        var createDto = new CreateOpportunityDto
        {
            Title = "Get Test Opportunity",
            Stage = "qualification",
            Value = 75000
        };
        var createResponse = await _client.PostAsJsonAsync("/api/opportunities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        var opportunityId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/opportunities/{opportunityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(opportunityId);
        result.Data.Title.Should().Be("Get Test Opportunity");
        result.Data.Stage.Should().Be("qualification");
    }

    [Fact]
    public async Task GetOpportunityById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/opportunities/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllOpportunities_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few opportunities
        for (int i = 1; i <= 3; i++)
        {
            var createDto = new CreateOpportunityDto
            {
                Title = $"Opportunity {i}",
                Stage = "prospecting",
                Value = 10000 * i
            };
            await _client.PostAsJsonAsync("/api/opportunities", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/opportunities?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<OpportunityDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateOpportunity_WithValidData_ReturnsUpdatedOpportunity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an opportunity first
        var createDto = new CreateOpportunityDto
        {
            Title = "Original Opportunity",
            Stage = "prospecting",
            Value = 50000,
            Probability = 20
        };
        var createResponse = await _client.PostAsJsonAsync("/api/opportunities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        var opportunityId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateOpportunityDto
        {
            Title = "Updated Opportunity",
            Stage = "proposal",
            Value = 75000,
            Probability = 60
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/opportunities/{opportunityId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Updated Opportunity");
        result.Data.Stage.Should().Be("proposal");
        result.Data.Value.Should().Be(75000);
        result.Data.Probability.Should().Be(60);
    }

    [Fact]
    public async Task UpdateOpportunity_DealStageProgression_UpdatesSuccessfully()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an opportunity
        var createDto = new CreateOpportunityDto
        {
            Title = "Stage Progression Test",
            Stage = "prospecting",
            Value = 100000
        };
        var createResponse = await _client.PostAsJsonAsync("/api/opportunities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        var opportunityId = createResult!.Data!.Id;

        // Progress through stages: prospecting -> qualification -> proposal -> negotiation -> closed-won
        var stages = new[] { "qualification", "proposal", "negotiation", "closed-won" };
        
        foreach (var stage in stages)
        {
            var updateDto = new UpdateOpportunityDto 
            { 
                Title = "Stage Progression Test",
                Stage = stage,
                Value = 100000
            };
            var response = await _client.PutAsJsonAsync($"/api/opportunities/{opportunityId}", updateDto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
            
            // Assert each stage update
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Data!.Stage.Should().Be(stage);
        }
    }

    [Fact]
    public async Task UpdateOpportunity_ValueAndProbabilityChange_UpdatesSuccessfully()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an opportunity
        var createDto = new CreateOpportunityDto
        {
            Title = "Value Test Opportunity",
            Stage = "prospecting",
            Value = 50000,
            Probability = 20
        };
        var createResponse = await _client.PostAsJsonAsync("/api/opportunities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        var opportunityId = createResult!.Data!.Id;

        // Update value and probability
        var updateDto = new UpdateOpportunityDto
        {
            Title = "Value Test Opportunity",
            Stage = "prospecting",
            Value = 150000,
            Probability = 80
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/opportunities/{opportunityId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        result!.Data!.Value.Should().Be(150000);
        result.Data.Probability.Should().Be(80);
    }

    [Fact]
    public async Task UpdateOpportunity_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateOpportunityDto
        {
            Title = "Updated Opportunity",
            Stage = "prospecting",
            Value = 10000
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/opportunities/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOpportunity_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an opportunity first
        var createDto = new CreateOpportunityDto
        {
            Title = "Opportunity To Delete",
            Stage = "prospecting",
            Value = 25000
        };
        var createResponse = await _client.PostAsJsonAsync("/api/opportunities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<OpportunityDto>>();
        var opportunityId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/opportunities/{opportunityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify opportunity is deleted
        var getResponse = await _client.GetAsync($"/api/opportunities/{opportunityId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteOpportunity_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/opportunities/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"opportunitytest_{Guid.NewGuid()}@test.com",
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
