using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class ActivityTests
{
    [Fact]
    public void Activity_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var activity = new Activity
        {
            Type = "call",
            Subject = "Important Call",
            Description = "Discuss project details",
            Status = "planned",
            Priority = "high",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Assert
        activity.Type.Should().Be("call");
        activity.Subject.Should().Be("Important Call");
        activity.Description.Should().Be("Discuss project details");
        activity.Status.Should().Be("planned");
        activity.Priority.Should().Be("high");
        activity.DueDate.Should().NotBeNull();
        activity.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Activity_MarkAsCompleted_UpdatesStatusAndDate()
    {
        // Arrange
        var activity = new Activity
        {
            Type = "task",
            Subject = "Test Task",
            Status = "in-progress",
            Priority = "medium"
        };

        // Act
        activity.Status = "completed";
        activity.CompletedDate = DateTime.UtcNow;

        // Assert
        activity.Status.Should().Be("completed");
        activity.CompletedDate.Should().NotBeNull();
        activity.CompletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Activity_WithCompanyId_AssociatesWithCompany()
    {
        // Arrange & Act
        var companyId = Guid.NewGuid();
        var activity = new Activity
        {
            Type = "meeting",
            Subject = "Company Meeting",
            Status = "planned",
            Priority = "high",
            CompanyId = companyId
        };

        // Assert
        activity.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void Activity_WithContactId_AssociatesWithContact()
    {
        // Arrange & Act
        var contactId = Guid.NewGuid();
        var activity = new Activity
        {
            Type = "email",
            Subject = "Follow-up Email",
            Status = "completed",
            Priority = "low",
            ContactId = contactId
        };

        // Assert
        activity.ContactId.Should().Be(contactId);
    }

    [Fact]
    public void Activity_WithLeadId_AssociatesWithLead()
    {
        // Arrange & Act
        var leadId = Guid.NewGuid();
        var activity = new Activity
        {
            Type = "call",
            Subject = "Lead Follow-up",
            Status = "planned",
            Priority = "high",
            LeadId = leadId
        };

        // Assert
        activity.LeadId.Should().Be(leadId);
    }

    [Fact]
    public void Activity_WithOpportunityId_AssociatesWithOpportunity()
    {
        // Arrange & Act
        var opportunityId = Guid.NewGuid();
        var activity = new Activity
        {
            Type = "meeting",
            Subject = "Deal Discussion",
            Status = "planned",
            Priority = "high",
            OpportunityId = opportunityId
        };

        // Assert
        activity.OpportunityId.Should().Be(opportunityId);
    }

    [Fact]
    public void Activity_WithAssignedUser_AssignsToUser()
    {
        // Arrange & Act
        var userId = Guid.NewGuid();
        var activity = new Activity
        {
            Type = "task",
            Subject = "Assigned Task",
            Status = "planned",
            Priority = "medium",
            AssignedUserId = userId
        };

        // Assert
        activity.AssignedUserId.Should().Be(userId);
    }

    [Fact]
    public void Activity_SoftDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var activity = new Activity
        {
            Type = "task",
            Subject = "Test Task",
            Status = "planned",
            Priority = "low"
        };

        // Act
        activity.IsDeleted = true;

        // Assert
        activity.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Activity_Deactivate_SetsIsActiveFalse()
    {
        // Arrange
        var activity = new Activity
        {
            Type = "call",
            Subject = "Old Call",
            Status = "completed",
            Priority = "low"
        };

        // Act
        activity.IsActive = false;

        // Assert
        activity.IsActive.Should().BeFalse();
    }
}
