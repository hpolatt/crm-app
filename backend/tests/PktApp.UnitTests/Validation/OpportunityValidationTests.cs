using System.ComponentModel.DataAnnotations;
using PktApp.Core.DTOs.Opportunities;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Validation;

public class OpportunityValidationTests
{
    private static IList<ValidationResult> ValidateDto(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CreateOpportunityDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Enterprise Sales Opportunity",
            Stage = "prospecting",
            Value = 150000,
            Probability = 50,
            Description = "Large enterprise deal",
            ExpectedCloseDate = DateTime.UtcNow.AddMonths(6)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithRequiredFieldsOnly_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Minimal Opportunity",
            Stage = "prospecting",
            Value = 0
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithoutTitle_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Stage = "prospecting",
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Title is required");
    }

    [Fact]
    public void CreateOpportunityDto_WithDefaultStage_IsValid()
    {
        // Note: Stage has default value "prospecting", so it's never null
        
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithTitleTooLong_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = new string('A', 201),
            Stage = "prospecting",
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Title cannot exceed 200 characters");
    }

    [Fact]
    public void CreateOpportunityDto_WithStageTooLong_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = new string('A', 51),
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Stage cannot exceed 50 characters");
    }

    [Fact]
    public void CreateOpportunityDto_WithNegativeValue_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "prospecting",
            Value = -1000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Value must be a positive number");
    }

    [Fact]
    public void CreateOpportunityDto_WithZeroValue_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "prospecting",
            Value = 0
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithProbabilityBetween0And100_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "qualification",
            Value = 50000,
            Probability = 75
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithProbabilityOver100_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "qualification",
            Value = 50000,
            Probability = 150
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Probability must be between 0 and 100");
    }

    [Fact]
    public void CreateOpportunityDto_WithNegativeProbability_IsInvalid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "qualification",
            Value = 50000,
            Probability = -10
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Probability must be between 0 and 100");
    }

    [Fact]
    public void CreateOpportunityDto_WithNullProbability_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            Title = "Test Opportunity",
            Stage = "prospecting",
            Value = 50000,
            Probability = null
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOpportunityDto_WithAllRelations_IsValid()
    {
        // Arrange
        var dto = new CreateOpportunityDto
        {
            LeadId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            Title = "Opportunity with Relations",
            Stage = "qualification",
            Value = 100000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateOpportunityDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new UpdateOpportunityDto
        {
            Title = "Updated Opportunity",
            Stage = "negotiation",
            Value = 200000,
            Probability = 80
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateOpportunityDto_WithoutTitle_IsInvalid()
    {
        // Arrange
        var dto = new UpdateOpportunityDto
        {
            Stage = "negotiation",
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Title is required");
    }

    [Fact]
    public void UpdateOpportunityDto_WithoutStage_IsInvalid()
    {
        // Arrange
        var dto = new UpdateOpportunityDto
        {
            Title = "Updated Opportunity",
            Value = 10000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Stage is required");
    }

    [Fact]
    public void UpdateOpportunityDto_WithNegativeValue_IsInvalid()
    {
        // Arrange
        var dto = new UpdateOpportunityDto
        {
            Title = "Updated Opportunity",
            Stage = "negotiation",
            Value = -5000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().ContainSingle();
        validationResults[0].ErrorMessage.Should().Be("Value must be a positive number");
    }
}
