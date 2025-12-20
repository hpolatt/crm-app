using System.ComponentModel.DataAnnotations;
using PktApp.Core.DTOs.Activities;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Validation;

public class ActivityValidationTests
{
    private static IList<ValidationResult> ValidateDto(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CreateActivityDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "call",
            Subject = "Follow-up call with client",
            Description = "Discuss project requirements",
            Status = "planned",
            Priority = "high",
            DueDate = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateActivityDto_WithRequiredFieldsOnly_IsValid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "meeting",
            Subject = "Team sync"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateActivityDto_WithoutType_IsInvalid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Subject = "Test Activity"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Type is required");
    }

    [Fact]
    public void CreateActivityDto_WithoutSubject_IsInvalid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "email"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Subject is required");
    }

    [Fact]
    public void CreateActivityDto_WithTypeTooLong_IsInvalid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = new string('A', 51),
            Subject = "Test Activity"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Type cannot exceed 50 characters");
    }

    [Fact]
    public void CreateActivityDto_WithSubjectTooLong_IsInvalid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "call",
            Subject = new string('A', 201)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Subject cannot exceed 200 characters");
    }

    [Fact]
    public void CreateActivityDto_WithDefaultStatusAndPriority_IsValid()
    {
        // Note: Status and Priority have default values
        
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "task",
            Subject = "Test Activity"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateActivityDto_WithAllRelations_IsValid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "meeting",
            Subject = "Comprehensive Activity",
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            LeadId = Guid.NewGuid(),
            OpportunityId = Guid.NewGuid(),
            AssignedUserId = Guid.NewGuid()
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateActivityDto_WithDueDateInPast_IsValid()
    {
        // Note: No validation prevents past dates
        
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "call",
            Subject = "Overdue Activity",
            DueDate = DateTime.UtcNow.AddDays(-5)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateActivityDto_WithCompanyAndContact_IsValid()
    {
        // Arrange
        var dto = new CreateActivityDto
        {
            Type = "email",
            Subject = "Client Communication",
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid()
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateActivityDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "meeting",
            Subject = "Updated Activity",
            Status = "completed",
            Priority = "low",
            CompletedDate = DateTime.UtcNow
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateActivityDto_WithoutType_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Subject = "Test Activity",
            Status = "in_progress",
            Priority = "medium"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Type is required");
    }

    [Fact]
    public void UpdateActivityDto_WithoutSubject_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "task",
            Status = "in_progress",
            Priority = "medium"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Subject is required");
    }

    [Fact]
    public void UpdateActivityDto_WithoutStatus_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "call",
            Subject = "Test Activity",
            Priority = "high"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Status is required");
    }

    [Fact]
    public void UpdateActivityDto_WithoutPriority_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "email",
            Subject = "Test Activity",
            Status = "completed"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Priority is required");
    }

    [Fact]
    public void UpdateActivityDto_WithStatusTooLong_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "meeting",
            Subject = "Test Activity",
            Status = new string('A', 51),
            Priority = "medium"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Status cannot exceed 50 characters");
    }

    [Fact]
    public void UpdateActivityDto_WithPriorityTooLong_IsInvalid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "task",
            Subject = "Test Activity",
            Status = "in_progress",
            Priority = new string('A', 51)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Priority cannot exceed 50 characters");
    }

    [Fact]
    public void UpdateActivityDto_WithCompletedDateAndStatus_IsValid()
    {
        // Arrange
        var dto = new UpdateActivityDto
        {
            Type = "call",
            Subject = "Completed Call",
            Status = "completed",
            Priority = "high",
            CompletedDate = DateTime.UtcNow
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }
}
