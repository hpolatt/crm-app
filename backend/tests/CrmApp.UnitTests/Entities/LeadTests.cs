using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class LeadTests
{
    [Fact]
    public void Lead_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var lead = new Lead
        {
            Title = "Enterprise Lead",
            Description = "Large enterprise potential customer",
            Status = "new",
            Source = "website",
            Value = 50000,
            Probability = 20
        };

        // Assert
        lead.Title.Should().Be("Enterprise Lead");
        lead.Description.Should().Be("Large enterprise potential customer");
        lead.Status.Should().Be("new");
        lead.Source.Should().Be("website");
        lead.Value.Should().Be(50000);
        lead.Probability.Should().Be(20);
    }

    [Fact]
    public void Lead_UpdateStatus_ChangesStatus()
    {
        // Arrange
        var lead = new Lead
        {
            Title = "New Lead",
            Status = "new"
        };

        // Act
        lead.Status = "qualified";

        // Assert
        lead.Status.Should().Be("qualified");
    }

    [Fact]
    public void Lead_WithProbability_AssignsProbability()
    {
        // Arrange & Act
        var lead = new Lead
        {
            Title = "High Value Lead",
            Status = "qualified",
            Value = 100000,
            Probability = 85
        };

        // Assert
        lead.Probability.Should().Be(85);
        lead.Value.Should().Be(100000);
    }

    [Fact]
    public void Lead_WithAssignedUser_AssignsToUser()
    {
        // Arrange & Act
        var userId = Guid.NewGuid();
        var lead = new Lead
        {
            Title = "Assigned Lead",
            Status = "contacted",
            AssignedUserId = userId
        };

        // Assert
        lead.AssignedUserId.Should().Be(userId);
    }

    [Fact]
    public void Lead_WithCompanyId_AssociatesWithCompany()
    {
        // Arrange & Act
        var companyId = Guid.NewGuid();
        var lead = new Lead
        {
            Title = "Company Lead",
            Status = "new",
            CompanyId = companyId
        };

        // Assert
        lead.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void Lead_WithContactId_AssociatesWithContact()
    {
        // Arrange & Act
        var contactId = Guid.NewGuid();
        var lead = new Lead
        {
            Title = "Contact Lead",
            Status = "contacted",
            ContactId = contactId
        };

        // Assert
        lead.ContactId.Should().Be(contactId);
    }

    [Fact]
    public void Lead_WithExpectedCloseDate_SetsDate()
    {
        // Arrange & Act
        var expectedDate = DateTime.UtcNow.AddMonths(2);
        var lead = new Lead
        {
            Title = "Scheduled Lead",
            Status = "qualified",
            ExpectedCloseDate = expectedDate
        };

        // Assert
        lead.ExpectedCloseDate.Should().NotBeNull();
        lead.ExpectedCloseDate.Should().BeCloseTo(expectedDate, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Lead_SoftDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var lead = new Lead
        {
            Title = "Delete Me",
            Status = "new"
        };

        // Act
        lead.IsDeleted = true;

        // Assert
        lead.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Lead_Deactivate_SetsIsActiveFalse()
    {
        // Arrange
        var lead = new Lead
        {
            Title = "Inactive Lead",
            Status = "lost"
        };

        // Act
        lead.IsActive = false;

        // Assert
        lead.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Lead_WithNullableFields_AllowsNullValues()
    {
        // Arrange & Act
        var lead = new Lead
        {
            Title = "Minimal Lead",
            Status = "new",
            Description = null,
            Source = null,
            Value = null,
            Probability = null,
            ExpectedCloseDate = null,
            Notes = null,
            CompanyId = null,
            ContactId = null,
            AssignedUserId = null
        };

        // Assert
        lead.Description.Should().BeNull();
        lead.Source.Should().BeNull();
        lead.Value.Should().BeNull();
        lead.Probability.Should().BeNull();
        lead.ExpectedCloseDate.Should().BeNull();
        lead.Notes.Should().BeNull();
        lead.CompanyId.Should().BeNull();
        lead.ContactId.Should().BeNull();
        lead.AssignedUserId.Should().BeNull();
    }

    [Fact]
    public void Lead_UpdateValue_ChangesValue()
    {
        // Arrange
        var lead = new Lead
        {
            Title = "Growing Lead",
            Status = "qualified",
            Value = 30000
        };

        // Act
        lead.Value = 50000;
        lead.Probability = 60;

        // Assert
        lead.Value.Should().Be(50000);
        lead.Probability.Should().Be(60);
    }
}
