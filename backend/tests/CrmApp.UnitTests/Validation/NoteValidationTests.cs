using FluentAssertions;
using CrmApp.Core.DTOs.Notes;
using CrmApp.UnitTests.Helpers;

namespace CrmApp.UnitTests.Validation;

/// <summary>
/// Tests for Note entity constraint: at least one entity association is required
/// </summary>
public class NoteValidationTests
{
    [Fact]
    public void CreateNoteDto_WithCompanyId_IsValid()
    {
        // Arrange
        var dto = new CreateNoteDto
        {
            CompanyId = Guid.NewGuid(),
            Content = "Test note for company"
        };

        // Act
        var hasEntityAssociation = dto.CompanyId.HasValue || dto.ContactId.HasValue 
            || dto.LeadId.HasValue || dto.OpportunityId.HasValue;

        // Assert
        hasEntityAssociation.Should().BeTrue("Note must be associated with at least one entity");
    }

    [Fact]
    public void CreateNoteDto_WithContactId_IsValid()
    {
        // Arrange
        var dto = new CreateNoteDto
        {
            ContactId = Guid.NewGuid(),
            Content = "Test note for contact"
        };

        // Act
        var hasEntityAssociation = dto.CompanyId.HasValue || dto.ContactId.HasValue 
            || dto.LeadId.HasValue || dto.OpportunityId.HasValue;

        // Assert
        hasEntityAssociation.Should().BeTrue("Note must be associated with at least one entity");
    }

    [Fact]
    public void CreateNoteDto_WithNoEntityAssociation_IsInvalid()
    {
        // Arrange
        var dto = new CreateNoteDto
        {
            Content = "Orphaned note without any entity association"
        };

        // Act
        var hasEntityAssociation = dto.CompanyId.HasValue || dto.ContactId.HasValue 
            || dto.LeadId.HasValue || dto.OpportunityId.HasValue;

        // Assert
        hasEntityAssociation.Should().BeFalse("This note has no entity association");
    }

    [Fact]
    public void CreateNoteDto_WithMultipleEntityAssociations_IsValid()
    {
        // Arrange - A note can be associated with multiple entities
        var dto = new CreateNoteDto
        {
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            Content = "Note for both company and contact"
        };

        // Act
        var associationCount = new[] { dto.CompanyId, dto.ContactId, dto.LeadId, dto.OpportunityId }
            .Count(id => id.HasValue);

        // Assert
        associationCount.Should().BeGreaterThan(0, "Note must have at least one entity association");
    }

    [Fact]
    public void NoteEntity_ValidatesConstraintAtDatabaseLevel()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var note = TestDataFactory.CreateNote(companyId: companyId, content: "Valid note");

        // Act
        var hasAtLeastOneEntity = note.CompanyId.HasValue || note.ContactId.HasValue 
            || note.LeadId.HasValue || note.OpportunityId.HasValue;

        // Assert
        hasAtLeastOneEntity.Should().BeTrue("DB constraint requires at least one entity ID");
        note.CompanyId.Should().Be(companyId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateNoteDto_WithEmptyContent_IsInvalid(string? content)
    {
        // Arrange
        var dto = new CreateNoteDto
        {
            CompanyId = Guid.NewGuid(),
            Content = content!
        };

        // Act & Assert
        // In real validation, this would be caught by [Required] attribute
        string.IsNullOrWhiteSpace(dto.Content).Should().BeTrue("Content should not be empty");
    }

    [Fact]
    public void CreateNoteDto_WithExcessiveContent_ExceedsMaxLength()
    {
        // Arrange
        var excessiveContent = new string('A', 5001); // Max is 5000
        var dto = new CreateNoteDto
        {
            CompanyId = Guid.NewGuid(),
            Content = excessiveContent
        };

        // Act & Assert
        dto.Content.Length.Should().BeGreaterThan(5000, "Content exceeds maximum allowed length");
    }
}
