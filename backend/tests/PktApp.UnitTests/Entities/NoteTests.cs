using PktApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Entities;

public class NoteTests
{
    [Fact]
    public void Note_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var note = new Note
        {
            Content = "Test note content",
            IsPinned = false
        };

        // Assert
        note.Content.Should().Be("Test note content");
        note.IsPinned.Should().BeFalse();
    }

    [Fact]
    public void Note_CanBeAssociatedWithCompany()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var note = new Note
        {
            Content = "Company note"
        };

        // Act
        note.CompanyId = companyId;

        // Assert
        note.CompanyId.Should().Be(companyId);
        note.ContactId.Should().BeNull();
        note.LeadId.Should().BeNull();
        note.OpportunityId.Should().BeNull();
    }

    [Fact]
    public void Note_CanBeAssociatedWithContact()
    {
        // Arrange
        var contactId = Guid.NewGuid();
        var note = new Note
        {
            Content = "Contact note"
        };

        // Act
        note.ContactId = contactId;

        // Assert
        note.ContactId.Should().Be(contactId);
        note.CompanyId.Should().BeNull();
        note.LeadId.Should().BeNull();
        note.OpportunityId.Should().BeNull();
    }

    [Fact]
    public void Note_CanBeAssociatedWithLead()
    {
        // Arrange
        var leadId = Guid.NewGuid();
        var note = new Note
        {
            Content = "Lead note"
        };

        // Act
        note.LeadId = leadId;

        // Assert
        note.LeadId.Should().Be(leadId);
        note.CompanyId.Should().BeNull();
        note.ContactId.Should().BeNull();
        note.OpportunityId.Should().BeNull();
    }

    [Fact]
    public void Note_CanBeAssociatedWithOpportunity()
    {
        // Arrange
        var opportunityId = Guid.NewGuid();
        var note = new Note
        {
            Content = "Opportunity note"
        };

        // Act
        note.OpportunityId = opportunityId;

        // Assert
        note.OpportunityId.Should().Be(opportunityId);
        note.CompanyId.Should().BeNull();
        note.ContactId.Should().BeNull();
        note.LeadId.Should().BeNull();
    }

    [Fact]
    public void Note_CanBeSoftDeleted()
    {
        // Arrange
        var note = new Note
        {
            Content = "Test note",
            IsDeleted = false
        };

        // Act
        note.IsDeleted = true;

        // Assert
        note.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Note_Update_ChangesContent()
    {
        // Arrange
        var note = new Note
        {
            Content = "Original content"
        };

        // Act
        note.Content = "Updated content";
        note.UpdatedAt = DateTime.UtcNow;

        // Assert
        note.Content.Should().Be("Updated content");
        note.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Note_CanBePinned()
    {
        // Arrange
        var note = new Note
        {
            Content = "Test note",
            IsPinned = false
        };

        // Act
        note.IsPinned = true;

        // Assert
        note.IsPinned.Should().BeTrue();
    }

    [Fact]
    public void Note_CanHaveNavigationProperties()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "Test Company",
            Email = "test@company.com"
        };

        var note = new Note
        {
            Content = "Company note",
            CompanyId = companyId,
            Company = company
        };

        // Assert
        note.Company.Should().NotBeNull();
        note.Company!.Name.Should().Be("Test Company");
    }
}
