using CrmApp.Core.Interfaces;
using CrmApp.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace CrmApp.UnitTests.Services;

public class CacheServiceTests
{
    private readonly Mock<IDistributedCache> _mockCache;
    private readonly ICacheService _cacheService;

    public CacheServiceTests()
    {
        _mockCache = new Mock<IDistributedCache>();
        _cacheService = new CacheService(_mockCache.Object);
    }

    [Fact]
    public async Task GetAsync_WhenDataExists_ReturnsDeserializedObject()
    {
        // Arrange
        var key = "test-key";
        var testData = new TestCacheObject { Id = 1, Name = "Test" };
        var serializedData = System.Text.Json.JsonSerializer.Serialize(testData);
        var bytes = System.Text.Encoding.UTF8.GetBytes(serializedData);

        _mockCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        // Act
        var result = await _cacheService.GetAsync<TestCacheObject>(key);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_WhenDataDoesNotExist_ReturnsNull()
    {
        // Arrange
        var key = "non-existent-key";
        _mockCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _cacheService.GetAsync<TestCacheObject>(key);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_WithDefaultExpiration_CallsSetAsync()
    {
        // Arrange
        var key = "test-key";
        var testData = new TestCacheObject { Id = 3, Name = "Set Test" };
        var serializedData = System.Text.Json.JsonSerializer.Serialize(testData);
        var bytes = System.Text.Encoding.UTF8.GetBytes(serializedData);

        // Act
        await _cacheService.SetAsync(key, testData);

        // Assert
        _mockCache.Verify(x => x.SetAsync(
            key,
            It.Is<byte[]>(b => System.Text.Encoding.UTF8.GetString(b) == serializedData),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromHours(1)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithCustomExpiration_UsesProvidedExpiration()
    {
        // Arrange
        var key = "test-key";
        var testData = new TestCacheObject { Id = 4, Name = "Custom Expiration" };
        var serializedData = System.Text.Json.JsonSerializer.Serialize(testData);
        var customExpiration = TimeSpan.FromMinutes(30);

        // Act
        await _cacheService.SetAsync(key, testData, customExpiration);

        // Assert
        _mockCache.Verify(x => x.SetAsync(
            key,
            It.Is<byte[]>(b => System.Text.Encoding.UTF8.GetString(b) == serializedData),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == customExpiration),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_WithNullExpiration_UsesDefaultExpiration()
    {
        // Arrange
        var key = "test-key";
        var testData = new TestCacheObject { Id = 5, Name = "Null Expiration" };
        var serializedData = System.Text.Json.JsonSerializer.Serialize(testData);

        // Act
        await _cacheService.SetAsync(key, testData, null);

        // Assert
        _mockCache.Verify(x => x.SetAsync(
            key,
            It.Is<byte[]>(b => System.Text.Encoding.UTF8.GetString(b) == serializedData),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromHours(1)),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_CallsDistributedCacheRemove()
    {
        // Arrange
        var key = "test-key";

        // Act
        await _cacheService.RemoveAsync(key);

        // Assert
        _mockCache.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_WhenDataExists_ReturnsTrue()
    {
        // Arrange
        var key = "existing-key";
        var testData = "some data";
        var bytes = System.Text.Encoding.UTF8.GetBytes(testData);

        _mockCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenDataDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var key = "non-existent-key";
        _mockCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WhenDataIsEmptyString_ReturnsFalse()
    {
        // Arrange
        var key = "empty-key";
        var emptyBytes = System.Text.Encoding.UTF8.GetBytes(string.Empty);
        _mockCache.Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyBytes);

        // Act
        var result = await _cacheService.ExistsAsync(key);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_SerializesObjectCorrectly()
    {
        // Arrange
        var key = "serialize-test";
        var testData = new TestCacheObject { Id = 99, Name = "Serialization Test" };
        byte[]? capturedData = null;

        _mockCache.Setup(x => x.SetAsync(
            key,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>(
                (k, data, opts, ct) => capturedData = data)
            .Returns(Task.CompletedTask);

        // Act
        await _cacheService.SetAsync(key, testData);

        // Assert
        capturedData.Should().NotBeNull();
        var serializedString = System.Text.Encoding.UTF8.GetString(capturedData!);
        var deserializedData = System.Text.Json.JsonSerializer.Deserialize<TestCacheObject>(serializedString);
        deserializedData.Should().NotBeNull();
        deserializedData!.Id.Should().Be(99);
        deserializedData.Name.Should().Be("Serialization Test");
    }

    // Test class for cache operations
    private class TestCacheObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
