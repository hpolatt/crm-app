using System.Net;
using System.Net.Http.Json;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Auth;
using CrmApp.Core.DTOs.Companies;
using CrmApp.Core.DTOs.Contacts;
using CrmApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace CrmApp.IntegrationTests.Controllers;

public class ContactsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ContactsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateContact_WithValidData_ReturnsCreatedContact()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Phone = "+1234567890",
            Position = "Sales Manager",
            Department = "Sales"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contacts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
        result.Data.Email.Should().Be("john.doe@test.com");
        result.Data.Position.Should().Be("Sales Manager");
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateContact_WithCompanyId_ReturnsContactWithCompany()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a company
        var companyDto = new CreateCompanyDto
        {
            Name = "Contact Test Company",
            Email = "contacttest@company.com"
        };
        var companyResponse = await _client.PostAsJsonAsync("/api/companies", companyDto);
        var companyResult = await companyResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = companyResult!.Data!.Id;

        // Create contact with company
        var createDto = new CreateContactDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@test.com",
            CompanyId = companyId,
            Position = "CEO"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contacts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.CompanyId.Should().Be(companyId);
        result.Data.CompanyName.Should().Be("Contact Test Company");
    }

    [Fact]
    public async Task CreateContact_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateContactDto
        {
            FirstName = "", // Required field empty
            LastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contacts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateContact_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/contacts", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetContactById_WithExistingId_ReturnsContact()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a contact
        var createDto = new CreateContactDto
        {
            FirstName = "Get",
            LastName = "Test",
            Email = "gettest@contact.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contacts", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        var contactId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/contacts/{contactId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(contactId);
        result.Data.FirstName.Should().Be("Get");
        result.Data.LastName.Should().Be("Test");
    }

    [Fact]
    public async Task GetContactById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/contacts/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllContacts_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few contacts
        for (int i = 1; i <= 3; i++)
        {
            var createDto = new CreateContactDto
            {
                FirstName = $"Contact{i}",
                LastName = $"Test{i}",
                Email = $"contact{i}@test.com"
            };
            await _client.PostAsJsonAsync("/api/contacts", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/contacts?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ContactDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task UpdateContact_WithValidData_ReturnsUpdatedContact()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a contact first
        var createDto = new CreateContactDto
        {
            FirstName = "Original",
            LastName = "Contact",
            Email = "original@contact.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contacts", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        var contactId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateContactDto
        {
            FirstName = "Updated",
            LastName = "Contact",
            Position = "Director",
            Department = "Marketing"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/contacts/{contactId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("Updated");
        result.Data.Position.Should().Be("Director");
        result.Data.Department.Should().Be("Marketing");
    }

    [Fact]
    public async Task UpdateContact_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateContactDto
        {
            FirstName = "Updated",
            LastName = "Test"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/contacts/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteContact_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a contact first
        var createDto = new CreateContactDto
        {
            FirstName = "Delete",
            LastName = "Me",
            Email = "delete@contact.com"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/contacts", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        var contactId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/contacts/{contactId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify contact is deleted
        var getResponse = await _client.GetAsync($"/api/contacts/{contactId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteContact_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/contacts/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"contacttest_{Guid.NewGuid()}@test.com",
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
