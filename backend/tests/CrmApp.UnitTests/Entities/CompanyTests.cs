using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class CompanyTests
{
    [Fact]
    public void Company_Creation_SetsPropertiesCorrectly()
    {
        // Arrange & Act
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            Phone = "+1234567890",
            Website = "https://testcompany.com",
            Industry = "Technology",
            Address = "123 Test St",
            City = "Test City",
            Country = "Test Country",
            PostalCode = "12345"
        };

        // Assert
        company.Name.Should().Be("Test Company");
        company.Email.Should().Be("test@company.com");
        company.Phone.Should().Be("+1234567890");
        company.Website.Should().Be("https://testcompany.com");
        company.Industry.Should().Be("Technology");
        company.Address.Should().Be("123 Test St");
        company.City.Should().Be("Test City");
        company.Country.Should().Be("Test Country");
        company.PostalCode.Should().Be("12345");
    }

    [Fact]
    public void Company_CanHaveMultipleContacts()
    {
        // Arrange
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com"
        };

        var contact1 = new Contact
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@test.com",
            CompanyId = company.Id
        };

        var contact2 = new Contact
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@test.com",
            CompanyId = company.Id
        };

        // Act
        company.Contacts = new List<Contact> { contact1, contact2 };

        // Assert
        company.Contacts.Should().HaveCount(2);
        company.Contacts.Should().Contain(c => c.FirstName == "John");
        company.Contacts.Should().Contain(c => c.FirstName == "Jane");
    }

    [Fact]
    public void Company_CanHaveMultipleOpportunities()
    {
        // Arrange
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com"
        };

        var opportunity1 = new Opportunity
        {
            Title = "Deal 1",
            Value = 10000,
            CompanyId = company.Id
        };

        var opportunity2 = new Opportunity
        {
            Title = "Deal 2",
            Value = 20000,
            CompanyId = company.Id
        };

        // Act
        company.Opportunities = new List<Opportunity> { opportunity1, opportunity2 };

        // Assert
        company.Opportunities.Should().HaveCount(2);
        company.Opportunities.Sum(o => o.Value).Should().Be(30000);
    }

    [Fact]
    public void Company_CanBeAssignedToUser()
    {
        // Arrange
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            Source = "Website"
        };

        // Act
        company.Source = "Referral";

        // Assert
        company.Source.Should().Be("Referral");
    }

    [Fact]
    public void Company_CanBeSoftDeleted()
    {
        // Arrange
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            IsDeleted = false
        };

        // Act
        company.IsDeleted = true;

        // Assert
        company.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Company_UpdateProperties_ChangesValues()
    {
        // Arrange
        var company = new Company
        {
            Name = "Old Name",
            Email = "old@company.com",
            Industry = "Old Industry"
        };

        // Act
        company.Name = "New Name";
        company.Email = "new@company.com";
        company.Industry = "New Industry";
        company.UpdatedAt = DateTime.UtcNow;

        // Assert
        company.Name.Should().Be("New Name");
        company.Email.Should().Be("new@company.com");
        company.Industry.Should().Be("New Industry");
        company.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Company_CanHaveNotesAsString()
    {
        // Arrange
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com"
        };

        // Act
        company.Notes = "Important note about this company";

        // Assert
        company.Notes.Should().Be("Important note about this company");
    }

    [Fact]
    public void Company_CanSetEmployeeCountAndRevenue()
    {
        // Arrange & Act
        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            EmployeeCount = 500,
            AnnualRevenue = 10000000.50m
        };

        // Assert
        company.EmployeeCount.Should().Be(500);
        company.AnnualRevenue.Should().Be(10000000.50m);
    }
}
