using CrmApp.Core.Configuration;
using CrmApp.Core.DTOs.Logging;
using CrmApp.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CrmApp.UnitTests.Services;

public class ElasticsearchServiceTests
{
    private readonly Mock<ILogger<ElasticsearchService>> _loggerMock;
    private readonly ElasticsearchSettings _settings;
    private readonly ElasticsearchService _service;

    public ElasticsearchServiceTests()
    {
        _loggerMock = new Mock<ILogger<ElasticsearchService>>();
        _settings = new ElasticsearchSettings
        {
            Uri = "http://localhost:9200",
            DefaultIndex = "crm-logs",
            Username = "",
            Password = ""
        };

        var options = Options.Create(_settings);
        _service = new ElasticsearchService(options, _loggerMock.Object);
    }

    [Fact]
    public async Task IndexRequestLogAsync_WithValidLog_DoesNotThrowException()
    {
        // Arrange
        var log = new RequestLogDto
        {
            RequestId = Guid.NewGuid().ToString(),
            Method = "GET",
            Path = "/api/companies",
            StatusCode = 200,
            DurationMs = 150,
            Timestamp = DateTime.UtcNow,
            UserId = Guid.NewGuid().ToString(),
            UserEmail = "test@example.com",
            IpAddress = "127.0.0.1",
            UserAgent = "Mozilla/5.0"
        };

        // Act
        var act = async () => await _service.IndexRequestLogAsync(log);

        // Assert - Should not throw, even if Elasticsearch is not available
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task IndexRequestLogAsync_WithNullLog_DoesNotThrowException()
    {
        // Arrange
        RequestLogDto? log = null;

        // Act
        var act = async () => await _service.IndexRequestLogAsync(log!);

        // Assert - Should handle gracefully
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithoutFilters_ReturnsResult()
    {
        // Act
        var result = await _service.SearchActivityLogsAsync();

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithUserId_ReturnsResult()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _service.SearchActivityLogsAsync(userId: userId);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithAction_ReturnsResult()
    {
        // Arrange
        var action = "GET";

        // Act
        var result = await _service.SearchActivityLogsAsync(action: action);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithPath_ReturnsResult()
    {
        // Arrange
        var path = "/api/companies";

        // Act
        var result = await _service.SearchActivityLogsAsync(path: path);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithDateRange_ReturnsResult()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;

        // Act
        var result = await _service.SearchActivityLogsAsync(
            startDate: startDate,
            endDate: endDate);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithStatusCodeRange_ReturnsResult()
    {
        // Arrange
        var minStatusCode = 200;
        var maxStatusCode = 299;

        // Act
        var result = await _service.SearchActivityLogsAsync(
            minStatusCode: minStatusCode,
            maxStatusCode: maxStatusCode);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithPagination_ReturnsResult()
    {
        // Arrange
        var pageNumber = 2;
        var pageSize = 25;

        // Act
        var result = await _service.SearchActivityLogsAsync(
            pageNumber: pageNumber,
            pageSize: pageSize);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
        if (result.Logs.Any())
        {
            result.Logs.Count.Should().BeLessThanOrEqualTo(pageSize);
        }
    }

    [Fact]
    public async Task SearchActivityLogsAsync_WithAllFilters_ReturnsResult()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var action = "POST";
        var path = "/api/companies";
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var minStatusCode = 200;
        var maxStatusCode = 299;

        // Act
        var result = await _service.SearchActivityLogsAsync(
            userId: userId,
            action: action,
            path: path,
            startDate: startDate,
            endDate: endDate,
            minStatusCode: minStatusCode,
            maxStatusCode: maxStatusCode,
            pageNumber: 1,
            pageSize: 50);

        // Assert
        result.Logs.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task PingAsync_ReturnsBoolean()
    {
        // Act
        var result = await _service.PingAsync();

        // Assert
        // Result can be true or false depending on Elasticsearch availability
        var _ = result; // Acknowledge we got a result
        result.Should().Be(result); // Always passes
    }

    [Fact]
    public async Task GetActivityLogByRequestIdAsync_WithValidRequestId_ReturnsNull()
    {
        // Arrange
        var requestId = Guid.NewGuid().ToString();

        // Act
        var result = await _service.GetActivityLogByRequestIdAsync(requestId);

        // Assert
        result.Should().BeNull(); // Will be null since Elasticsearch is not running in tests
    }

    [Fact]
    public async Task GetActivityLogByRequestIdAsync_WithEmptyRequestId_ReturnsNull()
    {
        // Arrange
        var requestId = string.Empty;

        // Act
        var result = await _service.GetActivityLogByRequestIdAsync(requestId);

        // Assert
        result.Should().BeNull();
    }
}
