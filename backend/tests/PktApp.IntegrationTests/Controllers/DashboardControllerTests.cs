using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Dashboard;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

[Collection("Sequential")]
public class DashboardControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetSummary_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/api/dashboard/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSummary_WithValidToken_ReturnsOkAndSummary()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetAuthToken(client);
        
        // Debug: Token should not be empty
        token.Should().NotBeNullOrEmpty();
        
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/dashboard/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardSummaryDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        // Verify summary structure
        result.Data!.TotalCompanies.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalContacts.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalLeads.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalOpportunities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.WinRate.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetSummary_VerifiesAllMetricsPresent()
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetAuthToken(client);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/dashboard/summary");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<DashboardSummaryDto>>();
        result!.Data.Should().NotBeNull();
        
        // Verify all expected properties are present
        result.Data!.TotalCompanies.Should().BeGreaterThanOrEqualTo(0);
        result.Data.ActiveCompanies.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalContacts.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalLeads.Should().BeGreaterThanOrEqualTo(0);
        result.Data.ActiveLeads.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalOpportunities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.OpenOpportunities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.WonOpportunities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.LostOpportunities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalOpportunityValue.Should().BeGreaterThanOrEqualTo(0);
        result.Data.WonOpportunityValue.Should().BeGreaterThanOrEqualTo(0);
        result.Data.OpenOpportunityValue.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalActivities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.OverdueActivities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TodayActivities.Should().BeGreaterThanOrEqualTo(0);
        result.Data.WinRate.Should().BeGreaterThanOrEqualTo(0);
        result.Data.AverageDealSize.Should().BeGreaterThanOrEqualTo(0);
    }

    private async Task<string> GetAuthToken(HttpClient client)
    {
        var registerDto = new
        {
            Email = $"dashboardtest_{Guid.NewGuid()}@test.com",
            Password = "Test123!",
            FirstName = "Test",
            LastName = "User",
            Phone = "+1234567890"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", registerDto);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        return result?.Data?.Token ?? string.Empty;
    }
}
