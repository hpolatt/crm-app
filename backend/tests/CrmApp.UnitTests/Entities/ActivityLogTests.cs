using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class ActivityLogTests
{
    [Fact]
    public void ActivityLog_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var userId = Guid.NewGuid();
        var activityLog = new ActivityLog
        {
            EntityType = "Company",
            EntityId = Guid.NewGuid(),
            Action = "Create",
            Description = "Created new company",
            UserId = userId,
            IpAddress = "192.168.1.1",
            UserAgent = "Mozilla/5.0"
        };

        // Assert
        activityLog.EntityType.Should().Be("Company");
        activityLog.Action.Should().Be("Create");
        activityLog.Description.Should().Be("Created new company");
        activityLog.UserId.Should().Be(userId);
        activityLog.IpAddress.Should().Be("192.168.1.1");
    }

    [Fact]
    public void ActivityLog_UpdateAction_LogsUpdate()
    {
        // Arrange & Act
        var entityId = Guid.NewGuid();
        var activityLog = new ActivityLog
        {
            EntityType = "Contact",
            EntityId = entityId,
            Action = "Update",
            Description = "Updated contact details"
        };

        // Assert
        activityLog.Action.Should().Be("Update");
        activityLog.EntityId.Should().Be(entityId);
    }

    [Fact]
    public void ActivityLog_DeleteAction_LogsDelete()
    {
        // Arrange & Act
        var activityLog = new ActivityLog
        {
            EntityType = "Lead",
            EntityId = Guid.NewGuid(),
            Action = "Delete",
            Description = "Deleted lead"
        };

        // Assert
        activityLog.Action.Should().Be("Delete");
        activityLog.EntityType.Should().Be("Lead");
    }

    [Fact]
    public void ActivityLog_WithDescription_StoresDetails()
    {
        // Arrange & Act
        var activityLog = new ActivityLog
        {
            EntityType = "Opportunity",
            EntityId = Guid.NewGuid(),
            Action = "Update",
            Description = "Changed stage from prospecting to qualification"
        };

        // Assert
        activityLog.Description.Should().Contain("prospecting");
        activityLog.Description.Should().Contain("qualification");
    }

    [Fact]
    public void ActivityLog_WithUserId_AssociatesToUser()
    {
        // Arrange & Act
        var userId = Guid.NewGuid();
        var activityLog = new ActivityLog
        {
            EntityType = "Company",
            EntityId = Guid.NewGuid(),
            Action = "Create",
            UserId = userId
        };

        // Assert
        activityLog.UserId.Should().Be(userId);
    }
}
