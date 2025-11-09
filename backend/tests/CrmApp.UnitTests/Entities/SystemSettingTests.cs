using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class SystemSettingTests
{
    [Fact]
    public void SystemSetting_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var setting = new SystemSetting
        {
            Key = "company.name",
            Value = "ACME Corporation",
            Description = "Company display name",
            Category = "General",
            DataType = "string"
        };

        // Assert
        setting.Key.Should().Be("company.name");
        setting.Value.Should().Be("ACME Corporation");
        setting.Category.Should().Be("General");
    }

    [Fact]
    public void SystemSetting_UpdateValue_ChangesValue()
    {
        // Arrange
        var setting = new SystemSetting
        {
            Key = "max.upload.size",
            Value = "10",
            DataType = "integer"
        };

        // Act
        setting.Value = "20";

        // Assert
        setting.Value.Should().Be("20");
    }

    [Fact]
    public void SystemSetting_BooleanType_StoresBoolean()
    {
        // Arrange & Act
        var setting = new SystemSetting
        {
            Key = "feature.enabled",
            Value = "true",
            DataType = "boolean"
        };

        // Assert
        setting.DataType.Should().Be("boolean");
        setting.Value.Should().Be("true");
    }

    [Fact]
    public void SystemSetting_WithCategory_GroupsSettings()
    {
        // Arrange & Act
        var setting = new SystemSetting
        {
            Key = "smtp.host",
            Value = "smtp.gmail.com",
            Category = "Email",
            DataType = "string"
        };

        // Assert
        setting.Category.Should().Be("Email");
    }

    [Fact]
    public void SystemSetting_WithDescription_ProvidesContext()
    {
        // Arrange & Act
        var setting = new SystemSetting
        {
            Key = "api.timeout",
            Value = "30",
            Description = "API request timeout in seconds",
            DataType = "integer"
        };

        // Assert
        setting.Description.Should().Contain("timeout");
        setting.Description.Should().Contain("seconds");
    }

    [Fact]
    public void SystemSetting_Deactivate_SetsIsActiveFalse()
    {
        // Arrange
        var setting = new SystemSetting
        {
            Key = "old.feature",
            Value = "enabled",
            IsActive = true
        };

        // Act
        setting.IsActive = false;

        // Assert
        setting.IsActive.Should().BeFalse();
    }
}
