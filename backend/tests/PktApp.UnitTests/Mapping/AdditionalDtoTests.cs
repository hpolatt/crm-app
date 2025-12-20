using PktApp.Core.DTOs.Roles;
using PktApp.Core.DTOs.Users;
using PktApp.Core.DTOs.Dashboard;
using PktApp.Core.DTOs.ActivityLogs;
using PktApp.Core.DTOs.Settings;
using PktApp.Core.DTOs;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Mapping;

/// <summary>
/// Additional DTO property tests for Roles, Users, Dashboard, etc.
/// </summary>
public class AdditionalDtoTests
{
    [Fact]
    public void CreateRoleRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateRoleRequest
        {
            Name = "Manager",
            Description = "Manager role with elevated privileges"
        };

        // Assert
        dto.Name.Should().Be("Manager");
        dto.Description.Should().Be("Manager role with elevated privileges");
    }

    [Fact]
    public void UpdateRoleRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new UpdateRoleRequest
        {
            Name = "Senior Manager",
            Description = "Updated description"
        };

        // Assert
        dto.Name.Should().Be("Senior Manager");
        dto.Description.Should().Be("Updated description");
    }

    [Fact]
    public void RoleDto_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new RoleDto
        {
            Id = Guid.NewGuid(),
            Name = "Administrator",
            Description = "Full system access",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Name.Should().Be("Administrator");
        dto.Description.Should().Be("Full system access");
        dto.IsActive.Should().BeTrue();
        dto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateUserRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new CreateUserRequest
        {
            Email = "newuser@company.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123!",
            Phone = "+1234567890",
            RoleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        };

        // Assert
        dto.Email.Should().Be("newuser@company.com");
        dto.FirstName.Should().Be("John");
        dto.LastName.Should().Be("Doe");
        dto.Password.Should().Be("SecurePass123!");
        dto.Phone.Should().Be("+1234567890");
        dto.RoleIds.Should().HaveCount(2);
    }

    [Fact]
    public void UpdateUserRequest_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new UpdateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Phone = "+9876543210",
            IsActive = true
        };

        // Assert
        dto.FirstName.Should().Be("Jane");
        dto.LastName.Should().Be("Smith");
        dto.Phone.Should().Be("+9876543210");
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UserDto_WithRoles_PropertiesWork()
    {
        // Arrange & Act
        var dto = new CrmApp.Core.DTOs.Users.UserDto
        {
            Id = Guid.NewGuid(),
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User",
            Phone = "+1112223333",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Roles = new List<string> { "Admin", "User" }
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Email.Should().Be("user@test.com");
        dto.IsActive.Should().BeTrue();
        dto.Roles.Should().HaveCount(2);
        dto.Roles.Should().Contain("Admin");
    }

    [Fact]
    public void DashboardSummaryDto_AllPropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new DashboardSummaryDto
        {
            TotalCompanies = 150,
            ActiveCompanies = 120,
            TotalContacts = 500,
            TotalLeads = 75,
            ActiveLeads = 45,
            TotalOpportunities = 30,
            OpenOpportunities = 12,
            WonOpportunities = 10,
            LostOpportunities = 8,
            TotalActivities = 200,
            OverdueActivities = 15,
            TodayActivities = 25,
            TotalOpportunityValue = 1500000.00m,
            WonOpportunityValue = 600000.00m,
            OpenOpportunityValue = 900000.00m,
            WinRate = 33.33,
            AverageDealSize = 50000.00m
        };

        // Assert
        dto.TotalCompanies.Should().Be(150);
        dto.ActiveCompanies.Should().Be(120);
        dto.TotalContacts.Should().Be(500);
        dto.TotalLeads.Should().Be(75);
        dto.ActiveLeads.Should().Be(45);
        dto.TotalOpportunities.Should().Be(30);
        dto.OpenOpportunities.Should().Be(12);
        dto.WonOpportunities.Should().Be(10);
        dto.TotalActivities.Should().Be(200);
        dto.OverdueActivities.Should().Be(15);
        dto.TodayActivities.Should().Be(25);
        dto.TotalOpportunityValue.Should().Be(1500000.00m);
        dto.WinRate.Should().Be(33.33);
        dto.AverageDealSize.Should().Be(50000.00m);
    }

    [Fact]
    public void ActivityLogDto_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new ActivityLogDto
        {
            Id = Guid.NewGuid(),
            EntityType = "Company",
            EntityId = Guid.NewGuid(),
            Action = "Created",
            UserId = Guid.NewGuid(),
            UserName = "John Doe",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.EntityType.Should().Be("Company");
        dto.EntityId.Should().NotBeEmpty();
        dto.Action.Should().Be("Created");
        dto.UserId.Should().NotBeEmpty();
        dto.UserName.Should().Be("John Doe");
        dto.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SystemSettingDto_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new SystemSettingDto
        {
            Id = Guid.NewGuid(),
            Key = "MaxLoginAttempts",
            Value = "5",
            Category = "Security",
            DataType = "Integer",
            Description = "Maximum failed login attempts before lockout",
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        dto.Id.Should().NotBeEmpty();
        dto.Key.Should().Be("MaxLoginAttempts");
        dto.Value.Should().Be("5");
        dto.Category.Should().Be("Security");
        dto.DataType.Should().Be("Integer");
        dto.Description.Should().Be("Maximum failed login attempts before lockout");
    }

    [Fact]
    public void UpdateSystemSettingDto_PropertiesCanBeSet()
    {
        // Arrange & Act
        var dto = new UpdateSystemSettingDto
        {
            Value = "10"
        };

        // Assert
        dto.Value.Should().Be("10");
    }

    [Fact]
    public void ApiResponse_Success_PropertiesWork()
    {
        // Arrange & Act
        var dto = new ApiResponse<string>
        {
            Success = true,
            Message = "Operation completed successfully",
            Data = "Test data"
        };

        // Assert
        dto.Success.Should().BeTrue();
        dto.Message.Should().Be("Operation completed successfully");
        dto.Data.Should().Be("Test data");
    }

    [Fact]
    public void ApiResponse_Failure_PropertiesWork()
    {
        // Arrange & Act
        var dto = new ApiResponse<object>
        {
            Success = false,
            Message = "Operation failed",
            Data = null
        };

        // Assert
        dto.Success.Should().BeFalse();
        dto.Message.Should().Be("Operation failed");
        dto.Data.Should().BeNull();
    }

    [Fact]
    public void ApiResponse_WithErrors_PropertiesWork()
    {
        // Arrange & Act
        var dto = new ApiResponse<int>
        {
            Success = false,
            Message = "Validation failed",
            Errors = new List<string> { "Email is required", "Password too short" }
        };

        // Assert
        dto.Success.Should().BeFalse();
        dto.Message.Should().Be("Validation failed");
        dto.Errors.Should().HaveCount(2);
        dto.Errors.Should().Contain("Email is required");
        dto.Errors.Should().Contain("Password too short");
    }

    [Fact]
    public void ApiResponse_GenericType_WorksWithComplexObjects()
    {
        // Arrange
        var userData = new { Name = "Test", Age = 30 };

        // Act
        var dto = new ApiResponse<object>
        {
            Success = true,
            Data = userData,
            Message = "User retrieved"
        };

        // Assert
        dto.Success.Should().BeTrue();
        dto.Data.Should().NotBeNull();
        dto.Message.Should().Be("User retrieved");
    }
}
