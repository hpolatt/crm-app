using CrmApp.Core.DTOs.DealStages;
using CrmApp.Core.DTOs.Logging;
using FluentAssertions;

namespace CrmApp.UnitTests.Mapping;

public class AdditionalCoreDtoTests
{
    [Fact]
    public void CreateDealStageDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new CreateDealStageDto
        {
            Name = "Prospecting",
            Order = 1,
            Description = "Initial contact stage",
            Color = "#FF5733",
            IsDefault = false,
            IsActive = true
        };

        // Assert
        dto.Name.Should().Be("Prospecting");
        dto.Order.Should().Be(1);
        dto.Description.Should().Be("Initial contact stage");
        dto.Color.Should().Be("#FF5733");
        dto.IsDefault.Should().BeFalse();
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateDealStageDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new UpdateDealStageDto
        {
            Name = "Qualified",
            Order = 2,
            Description = "Qualified lead stage",
            Color = "#33FF57",
            IsDefault = false,
            IsActive = true
        };

        // Assert
        dto.Name.Should().Be("Qualified");
        dto.Order.Should().Be(2);
        dto.Description.Should().Be("Qualified lead stage");
        dto.Color.Should().Be("#33FF57");
        dto.IsDefault.Should().BeFalse();
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DealStageDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new DealStageDto
        {
            Id = Guid.NewGuid(),
            Name = "Proposal",
            Order = 3,
            Description = "Proposal sent to client",
            Color = "#5733FF",
            IsDefault = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Name.Should().Be("Proposal");
        dto.Order.Should().Be(3);
        dto.Description.Should().Be("Proposal sent to client");
        dto.Color.Should().Be("#5733FF");
        dto.IsDefault.Should().BeFalse();
        dto.IsActive.Should().BeTrue();
        dto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        dto.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void RequestLogDto_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var dto = new RequestLogDto
        {
            Timestamp = DateTime.UtcNow,
            RequestId = Guid.NewGuid().ToString(),
            Method = "POST",
            Path = "/api/companies",
            QueryString = "?filter=active",
            StatusCode = 201,
            DurationMs = 250,
            UserId = Guid.NewGuid().ToString(),
            UserEmail = "admin@crm.com",
            IpAddress = "192.168.1.100",
            UserAgent = "Mozilla/5.0",
            RequestHeaders = new Dictionary<string, string>
            {
                ["Content-Type"] = "application/json",
                ["Authorization"] = "Bearer token"
            },
            RequestBody = "{\"name\":\"Test\"}",
            ResponseBody = "{\"id\":\"123\"}",
            ErrorMessage = null,
            StackTrace = null,
            CustomFields = new Dictionary<string, object>
            {
                ["Region"] = "US",
                ["Version"] = "1.0"
            }
        };

        // Assert
        dto.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        dto.RequestId.Should().NotBeNullOrEmpty();
        dto.Method.Should().Be("POST");
        dto.Path.Should().Be("/api/companies");
        dto.QueryString.Should().Be("?filter=active");
        dto.StatusCode.Should().Be(201);
        dto.DurationMs.Should().Be(250);
        dto.UserId.Should().NotBeNullOrEmpty();
        dto.UserEmail.Should().Be("admin@crm.com");
        dto.IpAddress.Should().Be("192.168.1.100");
        dto.UserAgent.Should().Be("Mozilla/5.0");
        dto.RequestHeaders.Should().HaveCount(2);
        dto.RequestHeaders["Content-Type"].Should().Be("application/json");
        dto.RequestBody.Should().Be("{\"name\":\"Test\"}");
        dto.ResponseBody.Should().Be("{\"id\":\"123\"}");
        dto.ErrorMessage.Should().BeNull();
        dto.StackTrace.Should().BeNull();
        dto.CustomFields.Should().HaveCount(2);
        dto.CustomFields["Region"].Should().Be("US");
    }

    [Fact]
    public void RequestLogDto_WithError_CanBeSet()
    {
        // Arrange & Act
        var dto = new RequestLogDto
        {
            Timestamp = DateTime.UtcNow,
            RequestId = Guid.NewGuid().ToString(),
            Method = "GET",
            Path = "/api/error",
            StatusCode = 500,
            DurationMs = 100,
            IpAddress = "127.0.0.1",
            UserAgent = "Test Client",
            ErrorMessage = "Internal Server Error",
            StackTrace = "at System.Exception..."
        };

        // Assert
        dto.StatusCode.Should().Be(500);
        dto.ErrorMessage.Should().Be("Internal Server Error");
        dto.StackTrace.Should().NotBeNullOrEmpty();
    }
}
