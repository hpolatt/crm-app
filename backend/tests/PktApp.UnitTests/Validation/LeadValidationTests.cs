using System.ComponentModel.DataAnnotations;
using PktApp.Core.DTOs.Leads;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Validation;

public class LeadValidationTests
{
    private static IList<ValidationResult> ValidateDto(object dto)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(dto);
        Validator.TryValidateObject(dto, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void CreateLeadDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "New Sales Lead",
            Description = "Potential customer from website",
            Source = "Website",
            Status = "new",
            Value = 50000,
            Probability = 30
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithMinimalData_IsValid()
    {
        // Note: No Required attributes on CreateLeadDto, so minimal data is valid
        
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Minimal Lead"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithAllOptionalFields_IsValid()
    {
        // Arrange
        var dto = new CreateLeadDto
        {
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            Title = "Complete Lead",
            Description = "Full lead information",
            Source = "Referral",
            Status = "qualified",
            Value = 100000,
            Probability = 75,
            ExpectedCloseDate = DateTime.UtcNow.AddMonths(3),
            AssignedUserId = Guid.NewGuid(),
            Notes = "Important prospect"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithNegativeValue_IsValid()
    {
        // Note: No validation prevents negative values
        
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Test Lead",
            Value = -1000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithProbabilityOver100_IsValid()
    {
        // Note: No Range validation on Probability
        
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Test Lead",
            Probability = 150
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithNegativeProbability_IsValid()
    {
        // Note: No Range validation on Probability
        
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Test Lead",
            Probability = -20
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithCompanyAndContact_IsValid()
    {
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Lead with Relations",
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid()
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateLeadDto_WithExpectedCloseDateInPast_IsValid()
    {
        // Note: No validation prevents past dates
        
        // Arrange
        var dto = new CreateLeadDto
        {
            Title = "Test Lead",
            ExpectedCloseDate = DateTime.UtcNow.AddMonths(-3)
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateLeadDto_WithValidData_IsValid()
    {
        // Arrange
        var dto = new UpdateLeadDto
        {
            Title = "Updated Lead Title",
            Status = "qualified",
            Value = 75000
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateLeadDto_WithPartialData_IsValid()
    {
        // Arrange
        var dto = new UpdateLeadDto
        {
            Status = "contacted"
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateLeadDto_WithEmptyTitle_IsValid()
    {
        // Note: No Required validation on UpdateLeadDto
        
        // Arrange
        var dto = new UpdateLeadDto
        {
            Title = string.Empty
        };

        // Act
        var validationResults = ValidateDto(dto);

        // Assert
        validationResults.Should().BeEmpty();
    }
}
