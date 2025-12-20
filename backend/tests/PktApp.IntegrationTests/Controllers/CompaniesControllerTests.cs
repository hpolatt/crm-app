using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Companies;
using PktApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class CompaniesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CompaniesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCompany_WithValidData_ReturnsCreatedCompany()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyDto
        {
            Name = "Test Company",
            Email = "test@company.com",
            Industry = "Technology",
            Website = "https://testcompany.com",
            Phone = "+1234567890",
            City = "Istanbul",
            Country = "Turkey"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Test Company");
        result.Data.Email.Should().Be("test@company.com");
        result.Data.Industry.Should().Be("Technology");
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateCompany_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateCompanyDto
        {
            Name = "", // Required field empty
            Email = "test@company.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCompany_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateCompanyDto
        {
            Name = "Test Company",
            Email = "test@company.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/companies", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCompanyById_WithExistingId_ReturnsCompany()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a company
        var createDto = new CreateCompanyDto
        {
            Name = "Get Test Company",
            Email = "gettest@company.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/companies", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/companies/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(companyId);
        result.Data.Name.Should().Be("Get Test Company");
        result.Data.Email.Should().Be("gettest@company.com");
    }

    [Fact]
    public async Task GetCompanyById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/companies/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllCompanies_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few companies
        for (int i = 1; i <= 3; i++)
        {
            var createDto = new CreateCompanyDto
            {
                Name = $"Company {i}",
                Email = $"company{i}@test.com"
            };
            await _client.PostAsJsonAsync("/api/companies", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/companies?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<CompanyDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateCompany_WithValidData_ReturnsUpdatedCompany()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a company first
        var createDto = new CreateCompanyDto
        {
            Name = "Original Company",
            Email = "original@company.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/companies", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Industry = "Finance",
            Website = "https://updated.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/companies/{companyId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Updated Company");
        result.Data.Industry.Should().Be("Finance");
        result.Data.Website.Should().Be("https://updated.com");
    }

    [Fact]
    public async Task UpdateCompany_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateCompanyDto
        {
            Name = "Updated Company"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/companies/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCompany_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a company first
        var createDto = new CreateCompanyDto
        {
            Name = "Company To Delete",
            Email = "delete@company.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/companies", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/companies/{companyId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify company is deleted
        var getResponse = await _client.GetAsync($"/api/companies/{companyId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteCompany_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/companies/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"companytest_{Guid.NewGuid()}@test.com",
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
