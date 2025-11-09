using CrmApp.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CrmApp.UnitTests.Entities;

public class OpportunityTests
{
    [Fact]
    public void Opportunity_WithValidData_CreatesSuccessfully()
    {
        // Arrange & Act
        var opportunity = new Opportunity
        {
            Title = "Enterprise Deal",
            Description = "Large enterprise software deal",
            Stage = "prospecting",
            Value = 50000,
            Probability = 25
        };

        // Assert
        opportunity.Title.Should().Be("Enterprise Deal");
        opportunity.Stage.Should().Be("prospecting");
        opportunity.Value.Should().Be(50000);
    }

    [Fact]
    public void Opportunity_UpdateStage_ChangesStage()
    {
        // Arrange
        var opportunity = new Opportunity
        {
            Title = "Test Deal",
            Stage = "prospecting",
            Value = 10000
        };

        // Act
        opportunity.Stage = "qualification";
        opportunity.Probability = 40;

        // Assert
        opportunity.Stage.Should().Be("qualification");
        opportunity.Probability.Should().Be(40);
    }

    [Fact]
    public void Opportunity_WithLeadId_AssociatesWithLead()
    {
        // Arrange & Act
        var leadId = Guid.NewGuid();
        var opportunity = new Opportunity
        {
            Title = "From Lead",
            Stage = "prospecting",
            Value = 25000,
            LeadId = leadId
        };

        // Assert
        opportunity.LeadId.Should().Be(leadId);
    }

    [Fact]
    public void Opportunity_SoftDelete_SetsIsDeletedTrue()
    {
        // Arrange
        var opportunity = new Opportunity
        {
            Title = "Delete Me",
            Stage = "prospecting",
            Value = 10000
        };

        // Act
        opportunity.IsDeleted = true;

        // Assert
        opportunity.IsDeleted.Should().BeTrue();
    }
}
