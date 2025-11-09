using System.Net;
using System.Net.Http.Json;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Auth;
using CrmApp.Core.DTOs.Notes;
using CrmApp.Core.DTOs.Companies;
using CrmApp.Core.DTOs.Contacts;
using CrmApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace CrmApp.IntegrationTests.Controllers;

public class NotesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public NotesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateNote_WithValidData_ReturnsCreatedNote()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateNoteDto
        {
            Content = "This is a test note about the meeting",
            IsPinned = false,
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/notes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Content.Should().Be("This is a test note about the meeting");
        result.Data.IsPinned.Should().BeFalse();
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateNote_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateNoteDto
        {
            Content = "Test note"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/notes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateNote_WithCompanyAssociation_AssociatesNoteWithCompany()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a company first
        var companyDto = new CreateCompanyDto
        {
            Name = "Test Company for Note",
            Email = "testnote@company.com",
            Industry = "Technology"
        };
        var companyResponse = await _client.PostAsJsonAsync("/api/companies", companyDto);
        var companyResult = await companyResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = companyResult!.Data!.Id;

        // Create note associated with company
        var createDto = new CreateNoteDto
        {
            CompanyId = companyId,
            Content = "Important note about this company",
            IsPinned = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/notes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        result!.Data!.CompanyId.Should().Be(companyId);
        result.Data.CompanyName.Should().Be("Test Company for Note");
        result.Data.IsPinned.Should().BeTrue();
    }

    [Fact]
    public async Task CreateNote_WithContactAssociation_AssociatesNoteWithContact()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a contact first
        var contactDto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = $"john.doe.{Guid.NewGuid()}@test.com"
        };
        var contactResponse = await _client.PostAsJsonAsync("/api/contacts", contactDto);
        var contactResult = await contactResponse.Content.ReadFromJsonAsync<ApiResponse<ContactDto>>();
        var contactId = contactResult!.Data!.Id;

        // Create note associated with contact
        var createDto = new CreateNoteDto
        {
            ContactId = contactId,
            Content = "Follow-up notes from conversation"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/notes", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        result!.Data!.ContactId.Should().Be(contactId);
        result.Data.ContactName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetNoteById_WithExistingId_ReturnsNote()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create a note
        var createDto = new CreateNoteDto
        {
            Content = "Test note for retrieval"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/notes", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        var noteId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/notes/{noteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(noteId);
        result.Data.Content.Should().Be("Test note for retrieval");
    }

    [Fact]
    public async Task GetNoteById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/notes/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllNotes_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few notes
        for (int i = 1; i <= 3; i++)
        {
            var createDto = new CreateNoteDto
            {
                Content = $"Note {i}",
                IsPinned = i == 1 // Pin the first note
            };
            await _client.PostAsJsonAsync("/api/notes", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/notes?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<NoteDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        
        // Verify pinned notes appear first
        result.Data.Items.First().IsPinned.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllNotes_WithCompanyFilter_ReturnsFilteredResults()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a company
        var companyDto = new CreateCompanyDto
        {
            Name = "Filter Test Company",
            Email = "filtertest@company.com",
            Industry = "Tech"
        };
        var companyResponse = await _client.PostAsJsonAsync("/api/companies", companyDto);
        var companyResult = await companyResponse.Content.ReadFromJsonAsync<ApiResponse<CompanyDto>>();
        var companyId = companyResult!.Data!.Id;

        // Create notes with and without company association
        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto
        {
            CompanyId = companyId,
            Content = "Company note 1"
        });

        await _client.PostAsJsonAsync("/api/notes", new CreateNoteDto
        {
            Content = "General note"
        });

        // Act
        var response = await _client.GetAsync($"/api/notes?companyId={companyId}&pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<NoteDto>>>();
        result!.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Should().OnlyContain(n => n.CompanyId == companyId);
    }

    [Fact]
    public async Task UpdateNote_WithValidData_ReturnsUpdatedNote()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a note first
        var createDto = new CreateNoteDto
        {
            Content = "Original note content",
            IsPinned = false
        };
        var createResponse = await _client.PostAsJsonAsync("/api/notes", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        var noteId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateNoteDto
        {
            Content = "Updated note content",
            IsPinned = true
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/notes/{noteId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Content.Should().Be("Updated note content");
        result.Data.IsPinned.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateNote_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateNoteDto
        {
            Content = "Updated content"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/notes/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNote_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a note first
        var createDto = new CreateNoteDto
        {
            Content = "Note to delete"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/notes", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<NoteDto>>();
        var noteId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/notes/{noteId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify note is deleted
        var getResponse = await _client.GetAsync($"/api/notes/{noteId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteNote_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/notes/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"notetest_{Guid.NewGuid()}@test.com",
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
