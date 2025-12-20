using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Reports;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class ReportsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ReportsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSalesReport_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/sales");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetSalesReport_WithValidToken_ReturnsOkAndReport()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/reports/sales");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<SalesReportDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        // Verify report structure
        result.Data!.MonthlySales.Should().NotBeNull();
        result.Data.SalesByStage.Should().NotBeNull();
        result.Data.SalesByUser.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSalesReport_WithDateRange_FiltersCorrectly()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var startDate = DateTime.UtcNow.AddMonths(-3);
        var endDate = DateTime.UtcNow;

        // Act
        var response = await _client.GetAsync($"/api/reports/sales?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<SalesReportDto>>();
        result!.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCustomerReport_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCustomerReport_WithValidToken_ReturnsOkAndReport()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/reports/customers");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CustomerReportDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        
        // Verify report structure
        result.Data!.CustomersBySource.Should().NotBeNull();
        result.Data.CustomersByIndustry.Should().NotBeNull();
    }

    [Fact]
    public async Task GetActivityReport_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/activities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetActivityReport_WithValidToken_ReturnsOkAndReport()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/reports/activities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetLeadReport_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/reports/leads");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetLeadReport_WithValidToken_ReturnsOkAndReport()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/reports/leads");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"reportstest_{Guid.NewGuid()}@test.com",
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
