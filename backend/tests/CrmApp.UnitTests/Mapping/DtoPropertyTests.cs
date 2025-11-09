using CrmApp.Core.DTOs.Companies;
using CrmApp.Core.DTOs.Contacts;
using CrmApp.Core.DTOs.Leads;
using CrmApp.Core.DTOs.Opportunities;
using CrmApp.Core.DTOs.Activities;
using CrmApp.Core.DTOs.Notes;
using CrmApp.Core.DTOs.Auth;
using CrmApp.Core.DTOs.Common;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Mapping;

/// <summary>
/// Tests for DTO property assignments to increase Core coverage
/// </summary>
public class DtoPropertyTests
{
    [Fact]
    public void CreateCompanyDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateCompanyDto
        {
            Name = "Test Company",
            Industry = "Tech",
            Website = "https://test.com",
            Phone = "+1234567890",
            Email = "test@company.com",
            Address = "123 Test St",
            City = "Test City",
            Country = "Test Country",
            PostalCode = "12345",
            Source = "Website",
            EmployeeCount = 100,
            AnnualRevenue = 1000000.50m,
            Notes = "Test notes"
        };

        // Assert - Verify all properties
        dto.Name.Should().Be("Test Company");
        dto.Industry.Should().Be("Tech");
        dto.Website.Should().Be("https://test.com");
        dto.Email.Should().Be("test@company.com");
        dto.EmployeeCount.Should().Be(100);
        dto.AnnualRevenue.Should().Be(1000000.50m);
    }

    [Fact]
    public void UpdateCompanyDto_PropertiesCanBeModified()
    {
        // Arrange & Act
        var dto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            Industry = "Finance",
            Email = "updated@company.com"
        };

        // Assert
        dto.Name.Should().Be("Updated Company");
        dto.Industry.Should().Be("Finance");
        dto.Email.Should().Be("updated@company.com");
    }

    [Fact]
    public void CompanyDto_ReadPropertiesWork()
    {
        // Arrange & Act
        var dto = new CompanyDto
        {
            Id = Guid.NewGuid(),
            Name = "DTO Company",
            Email = "dto@company.com",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Name.Should().Be("DTO Company");
        dto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateContactDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateContactDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            Phone = "+1234567890",
            Mobile = "+9876543210",
            CompanyId = Guid.NewGuid()
        };

        // Assert
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Email.Should().Be("john@doe.com");
        dto.CompanyId.Should().NotBeEmpty();
    }

    [Fact]
    public void UpdateContactDto_PropertiesCanBeModified()
    {
        // Arrange & Act
        var dto = new UpdateContactDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@smith.com",
            Phone = "+1112223333"
        };

        // Assert
        dto.FirstName.Should().Be("Jane");
        dto.LastName.Should().Be("Smith");
        dto.Email.Should().Be("jane@smith.com");
    }

    [Fact]
    public void CreateLeadDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateLeadDto
        {
            Title = "New Lead",
            Source = "Website",
            Status = "new",
            Value = 5000.00m
        };

        // Assert
        dto.Title.Should().Be("New Lead");
        dto.Source.Should().Be("Website");
        dto.Status.Should().Be("new");
        dto.Value.Should().Be(5000.00m);
    }

    [Fact]
    public void UpdateLeadDto_PropertiesCanBeModified()
    {
        // Arrange & Act
        var dto = new UpdateLeadDto
        {
            Title = "Updated Lead",
            Status = "qualified",
            Value = 7500.00m
        };

        // Assert
        dto.Title.Should().Be("Updated Lead");
        dto.Status.Should().Be("qualified");
        dto.Value.Should().Be(7500.00m);
    }

    [Fact]
    public void CreateOpportunityDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateOpportunityDto
        {
            Title = "Big Deal",
            Value = 100000.00m,
            Stage = "proposal",
            Probability = 75,
            CompanyId = Guid.NewGuid()
        };

        // Assert
        dto.Title.Should().Be("Big Deal");
        dto.Value.Should().Be(100000.00m);
        dto.Stage.Should().Be("proposal");
        dto.Probability.Should().Be(75);
        dto.CompanyId.Should().NotBeEmpty();
    }

    [Fact]
    public void UpdateOpportunityDto_PropertiesCanBeModified()
    {
        // Arrange & Act
        var dto = new UpdateOpportunityDto
        {
            Title = "Updated Deal",
            Value = 150000.00m,
            Stage = "negotiation",
            Probability = 90
        };

        // Assert
        dto.Title.Should().Be("Updated Deal");
        dto.Value.Should().Be(150000.00m);
        dto.Probability.Should().Be(90);
    }

    [Fact]
    public void CreateActivityDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateActivityDto
        {
            Type = "Meeting",
            Subject = "Client Meeting",
            Description = "Discuss contract",
            Status = "planned",
            Priority = "high",
            DueDate = DateTime.UtcNow.AddDays(7),
            CompanyId = Guid.NewGuid(),
            AssignedUserId = Guid.NewGuid()
        };

        // Assert
        dto.Type.Should().Be("Meeting");
        dto.Subject.Should().Be("Client Meeting");
        dto.Status.Should().Be("planned");
        dto.Priority.Should().Be("high");
        dto.DueDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateActivityDto_PropertiesCanBeModified()
    {
        // Arrange & Act
        var dto = new UpdateActivityDto
        {
            Type = "Call",
            Subject = "Follow-up Call",
            Status = "completed",
            Priority = "medium"
        };

        // Assert
        dto.Type.Should().Be("Call");
        dto.Subject.Should().Be("Follow-up Call");
        dto.Status.Should().Be("completed");
    }

    [Fact]
    public void CreateNoteDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateNoteDto
        {
            Content = "Important note",
            CompanyId = Guid.NewGuid(),
            IsPinned = true
        };

        // Assert
        dto.Content.Should().Be("Important note");
        dto.CompanyId.Should().NotBeEmpty();
        dto.IsPinned.Should().BeTrue();
    }

    [Fact]
    public void LoginRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new LoginRequest
        {
            Email = "user@test.com",
            Password = "SecurePass123!"
        };

        // Assert
        dto.Email.Should().Be("user@test.com");
        dto.Password.Should().Be("SecurePass123!");
    }

    [Fact]
    public void RegisterRequest_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new RegisterRequest
        {
            Email = "newuser@test.com",
            Password = "NewPass123!",
            FirstName = "New",
            LastName = "User",
            Phone = "+1234567890"
        };

        // Assert
        dto.Email.Should().Be("newuser@test.com");
        dto.Password.Should().Be("NewPass123!");
        dto.FirstName.Should().Be("New");
        dto.LastName.Should().Be("User");
        dto.Phone.Should().Be("+1234567890");
    }

    [Fact]
    public void LoginResponse_PropertiesCanBeSet()
    {
        // Arrange & Act
        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User",
            Roles = new List<string> { "User" }
        };

        var dto = new LoginResponse
        {
            Token = "jwt_token_here",
            RefreshToken = "refresh_token_here",
            User = userDto
        };

        // Assert
        dto.Token.Should().Be("jwt_token_here");
        dto.RefreshToken.Should().Be("refresh_token_here");
        dto.User.Should().NotBeNull();
        dto.User.Email.Should().Be("user@test.com");
    }

    [Fact]
    public void UserDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FirstName = "John",
            LastName = "Doe",
            Roles = new List<string> { "Admin", "User" }
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Email.Should().Be("user@test.com");
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Roles.Should().HaveCount(2);
        dto.Roles.Should().Contain("Admin");
    }

    [Fact]
    public void PagedResult_CalculatesPropertiesCorrectly()
    {
        // Arrange & Act
        var dto = new PagedResult<string>
        {
            Items = new List<string> { "Item1", "Item2", "Item3" },
            TotalCount = 100,
            PageNumber = 2,
            PageSize = 10
        };

        // Assert
        dto.Items.Should().HaveCount(3);
        dto.TotalCount.Should().Be(100);
        dto.PageNumber.Should().Be(2);
        dto.PageSize.Should().Be(10);
        dto.TotalPages.Should().Be(10); // 100 / 10
        dto.HasPreviousPage.Should().BeTrue();
        dto.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void PagedResult_FirstPage_HasNoPreviousPage()
    {
        // Arrange & Act
        var dto = new PagedResult<int>
        {
            Items = new List<int> { 1, 2, 3 },
            TotalCount = 50,
            PageNumber = 1,
            PageSize = 10
        };

        // Assert
        dto.HasPreviousPage.Should().BeFalse();
        dto.HasNextPage.Should().BeTrue();
    }

    [Fact]
    public void PagedResult_LastPage_HasNoNextPage()
    {
        // Arrange & Act
        var dto = new PagedResult<int>
        {
            Items = new List<int> { 1, 2, 3 },
            TotalCount = 23,
            PageNumber = 3,
            PageSize = 10
        };

        // Assert
        dto.HasPreviousPage.Should().BeTrue();
        dto.HasNextPage.Should().BeFalse();
        dto.TotalPages.Should().Be(3); // Ceiling(23/10) = 3
    }

    [Fact]
    public void PaginationQuery_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new PaginationQuery
        {
            PageNumber = 3,
            PageSize = 25
        };

        // Assert
        dto.PageNumber.Should().Be(3);
        dto.PageSize.Should().Be(25);
    }

    [Fact]
    public void RefreshTokenRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new RefreshTokenRequest
        {
            RefreshToken = "sample_refresh_token"
        };

        // Assert
        dto.RefreshToken.Should().Be("sample_refresh_token");
    }
}
