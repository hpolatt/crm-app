using System.Net;
using System.Net.Http.Json;
using PktApp.Core.DTOs;
using PktApp.Core.DTOs.Auth;
using PktApp.Core.DTOs.Activities;
using PktApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace PktApp.IntegrationTests.Controllers;

public class ActivitiesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ActivitiesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateActivity_WithValidData_ReturnsCreatedActivity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createDto = new CreateActivityDto
        {
            Type = "call",
            Subject = "Follow-up Call",
            Description = "Call to discuss proposal",
            Status = "planned",
            Priority = "high",
            DueDate = DateTime.UtcNow.AddDays(2),
            IsActive = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/activities", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Type.Should().Be("call");
        result.Data.Subject.Should().Be("Follow-up Call");
        result.Data.Status.Should().Be("planned");
        result.Data.Priority.Should().Be("high");
        result.Data.IsActive.Should().BeTrue();
        result.Data.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CreateActivity_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var createDto = new CreateActivityDto
        {
            Type = "meeting",
            Subject = "Test Meeting",
            Status = "planned",
            Priority = "medium"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/activities", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetActivityById_WithExistingId_ReturnsActivity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // First create an activity
        var createDto = new CreateActivityDto
        {
            Type = "email",
            Subject = "Send Proposal",
            Status = "planned",
            Priority = "high"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/activities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        var activityId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/activities/{activityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(activityId);
        result.Data.Subject.Should().Be("Send Proposal");
    }

    [Fact]
    public async Task GetActivityById_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/activities/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAllActivities_ReturnsPagedResult()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create a few activities
        var types = new[] { "call", "meeting", "email" };
        for (int i = 0; i < 3; i++)
        {
            var createDto = new CreateActivityDto
            {
                Type = types[i],
                Subject = $"Activity {i + 1}",
                Status = "planned",
                Priority = "medium"
            };
            await _client.PostAsJsonAsync("/api/activities", createDto);
        }

        // Act
        var response = await _client.GetAsync("/api/activities?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ActivityDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Count.Should().BeGreaterThanOrEqualTo(3);
        result.Data.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public async Task GetAllActivities_WithTypeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create activities with different types
        await _client.PostAsJsonAsync("/api/activities", new CreateActivityDto
        {
            Type = "call",
            Subject = "Call Activity",
            Status = "planned",
            Priority = "high"
        });

        await _client.PostAsJsonAsync("/api/activities", new CreateActivityDto
        {
            Type = "meeting",
            Subject = "Meeting Activity",
            Status = "planned",
            Priority = "medium"
        });

        // Act
        var response = await _client.GetAsync("/api/activities?type=call&pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResult<ActivityDto>>>();
        result!.Data!.Items.Should().NotBeEmpty();
        result.Data.Items.Should().OnlyContain(a => a.Type == "call");
    }

    [Fact]
    public async Task UpdateActivity_WithValidData_ReturnsUpdatedActivity()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an activity first
        var createDto = new CreateActivityDto
        {
            Type = "task",
            Subject = "Original Task",
            Status = "planned",
            Priority = "low"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/activities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        var activityId = createResult!.Data!.Id;

        // Prepare update
        var updateDto = new UpdateActivityDto
        {
            Type = "task",
            Subject = "Updated Task",
            Status = "planned",
            Priority = "high"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/activities/{activityId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Subject.Should().Be("Updated Task");
        result.Data.Priority.Should().Be("high");
    }

    [Fact]
    public async Task UpdateActivity_ChangeStatus_UpdatesSuccessfully()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an activity
        var createDto = new CreateActivityDto
        {
            Type = "call",
            Subject = "Important Call",
            Status = "planned",
            Priority = "high"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/activities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        var activityId = createResult!.Data!.Id;

        // Progress through statuses: planned -> in-progress -> completed
        var statuses = new[] { "in-progress", "completed" };
        
        foreach (var status in statuses)
        {
            var updateDto = new UpdateActivityDto 
            { 
                Type = "call",
                Subject = "Important Call",
                Status = status,
                Priority = "high"
            };
            var response = await _client.PutAsJsonAsync($"/api/activities/{activityId}", updateDto);
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
            
            // Assert each status update
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result!.Data!.Status.Should().Be(status);
        }
    }

    [Fact]
    public async Task UpdateActivity_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        var updateDto = new UpdateActivityDto
        {
            Type = "task",
            Subject = "Updated Activity",
            Status = "planned",
            Priority = "medium"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/activities/{nonExistentId}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteActivity_WithExistingId_ReturnsSuccess()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Create an activity first
        var createDto = new CreateActivityDto
        {
            Type = "task",
            Subject = "Activity To Delete",
            Status = "planned",
            Priority = "low"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/activities", createDto);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponse<ActivityDto>>();
        var activityId = createResult!.Data!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/activities/{activityId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify activity is deleted
        var getResponse = await _client.GetAsync($"/api/activities/{activityId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteActivity_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthToken();
        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/activities/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<string> GetAuthToken()
    {
        var registerDto = new
        {
            Email = $"activitytest_{Guid.NewGuid()}@test.com",
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
