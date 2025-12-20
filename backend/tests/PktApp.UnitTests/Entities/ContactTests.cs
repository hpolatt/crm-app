using PktApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Entities;

public class ContactTests
{
    [Fact]
    public void Contact_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var contact = new Contact
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+1234567890",
            Position = "Manager",
            Department = "Sales"
        };

        // Assert
        contact.FirstName.Should().Be("John");
        contact.LastName.Should().Be("Doe");
        contact.Email.Should().Be("john.doe@example.com");
        contact.Phone.Should().Be("+1234567890");
        contact.Position.Should().Be("Manager");
        contact.Department.Should().Be("Sales");
    }

    [Fact]
    public void Contact_WithCompanyId_AssociatesWithCompany()
    {
        // Arrange & Act
        var companyId = Guid.NewGuid();
        var contact = new Contact
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@company.com",
            CompanyId = companyId
        };

        // Assert
        contact.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public void Contact_SoftDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "Test",
            LastName = "Contact",
            Email = "test@contact.com"
        };

        // Act
        contact.IsDeleted = true;

        // Assert
        contact.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Contact_Deactivate_SetsIsActiveFalse()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "Old",
            LastName = "Contact",
            Email = "old@contact.com"
        };

        // Act
        contact.IsActive = false;

        // Assert
        contact.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Contact_UpdateDetails_UpdatesSuccessfully()
    {
        // Arrange
        var contact = new Contact
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@old.com",
            Position = "Junior"
        };

        // Act
        contact.Email = "john@new.com";
        contact.Position = "Senior Manager";
        contact.Phone = "+9876543210";

        // Assert
        contact.Email.Should().Be("john@new.com");
        contact.Position.Should().Be("Senior Manager");
        contact.Phone.Should().Be("+9876543210");
    }

    [Fact]
    public void Contact_WithNullableFields_AllowsNullValues()
    {
        // Arrange & Act
        var contact = new Contact
        {
            FirstName = "Minimal",
            LastName = "Contact",
            Email = null,
            Phone = null,
            Mobile = null,
            Position = null,
            Department = null,
            Address = null,
            City = null,
            Country = null
        };

        // Assert
        contact.Email.Should().BeNull();
        contact.Phone.Should().BeNull();
        contact.Mobile.Should().BeNull();
        contact.Position.Should().BeNull();
        contact.Department.Should().BeNull();
        contact.Address.Should().BeNull();
        contact.City.Should().BeNull();
        contact.Country.Should().BeNull();
    }
}
