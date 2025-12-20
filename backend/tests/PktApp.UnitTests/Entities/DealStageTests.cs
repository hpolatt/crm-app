using PktApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace PktApp.UnitTests.Entities;

public class DealStageTests
{
    [Fact]
    public void DealStage_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var dealStage = new DealStage
        {
            Name = "Qualification",
            Order = 1,
            Color = "#FF5733",
            Description = "Qualify the lead",
            IsActive = true
        };

        // Assert
        dealStage.Name.Should().Be("Qualification");
        dealStage.Order.Should().Be(1);
        dealStage.Color.Should().Be("#FF5733");
        dealStage.IsActive.Should().BeTrue();
    }

    [Fact]
    public void DealStage_UpdateOrder_ChangesOrder()
    {
        // Arrange
        var dealStage = new DealStage
        {
            Name = "Proposal",
            Order = 3
        };

        // Act
        dealStage.Order = 4;

        // Assert
        dealStage.Order.Should().Be(4);
    }

    [Fact]
    public void DealStage_Deactivate_SetsIsActiveFalse()
    {
        // Arrange
        var dealStage = new DealStage
        {
            Name = "Old Stage",
            Order = 5,
            IsActive = true
        };

        // Act
        dealStage.IsActive = false;

        // Assert
        dealStage.IsActive.Should().BeFalse();
    }

    [Fact]
    public void DealStage_SoftDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var dealStage = new DealStage
        {
            Name = "Deprecated",
            Order = 10
        };

        // Act
        dealStage.IsDeleted = true;

        // Assert
        dealStage.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void DealStage_WithColor_SetsColor()
    {
        // Arrange & Act
        var dealStage = new DealStage
        {
            Name = "Negotiation",
            Order = 4,
            Color = "#4CAF50"
        };

        // Assert
        dealStage.Color.Should().Be("#4CAF50");
    }
}
