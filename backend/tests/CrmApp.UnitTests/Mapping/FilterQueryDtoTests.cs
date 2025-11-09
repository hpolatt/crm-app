using CrmApp.Core.DTOs.Activities;
using CrmApp.Core.DTOs.Companies;
using CrmApp.Core.DTOs.Contacts;
using CrmApp.Core.DTOs.Leads;
using CrmApp.Core.DTOs.Notes;
using CrmApp.Core.DTOs.Opportunities;
using FluentAssertions;

namespace CrmApp.UnitTests.Mapping;

public class FilterQueryDtoTests
{
    [Fact]
    public void CompanyFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new CompanyFilterQuery
        {
            Name = "Tech Corp",
            Industry = "Software",
            Source = "Web",
            IsActive = true,
            CreatedFrom = DateTime.UtcNow.AddDays(-30),
            CreatedTo = DateTime.UtcNow,
            SearchTerm = "tech"
        };

        // Assert
        query.Name.Should().Be("Tech Corp");
        query.Industry.Should().Be("Software");
        query.Source.Should().Be("Web");
        query.IsActive.Should().BeTrue();
        query.CreatedFrom.Should().BeCloseTo(DateTime.UtcNow.AddDays(-30), TimeSpan.FromSeconds(1));
        query.CreatedTo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        query.SearchTerm.Should().Be("tech");
    }

    [Fact]
    public void ContactFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new ContactFilterQuery
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@company.com",
            Phone = "+1234567890",
            CompanyId = Guid.NewGuid(),
            IsActive = true,
            SearchTerm = "john",
            CreatedFrom = DateTime.UtcNow.AddDays(-60),
            CreatedTo = DateTime.UtcNow
        };

        // Assert
        query.FirstName.Should().Be("John");
        query.LastName.Should().Be("Doe");
        query.Email.Should().Be("john@company.com");
        query.Phone.Should().Be("+1234567890");
        query.CompanyId.Should().NotBeEmpty();
        query.IsActive.Should().BeTrue();
        query.SearchTerm.Should().Be("john");
        query.CreatedFrom.Should().BeCloseTo(DateTime.UtcNow.AddDays(-60), TimeSpan.FromSeconds(1));
        query.CreatedTo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void LeadFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new LeadFilterQuery
        {
            Title = "New Lead",
            Status = "new",
            Source = "Referral",
            IsActive = true,
            AssignedUserId = Guid.NewGuid(),
            SearchTerm = "lead",
            CreatedFrom = DateTime.UtcNow.AddDays(-15),
            CreatedTo = DateTime.UtcNow
        };

        // Assert
        query.Title.Should().Be("New Lead");
        query.Status.Should().Be("new");
        query.Source.Should().Be("Referral");
        query.IsActive.Should().BeTrue();
        query.AssignedUserId.Should().NotBeEmpty();
        query.SearchTerm.Should().Be("lead");
        query.CreatedFrom.Should().BeCloseTo(DateTime.UtcNow.AddDays(-15), TimeSpan.FromSeconds(1));
        query.CreatedTo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ActivityFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new ActivityFilterQuery
        {
            Type = "Call",
            Status = "completed",
            AssignedUserId = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            LeadId = Guid.NewGuid(),
            OpportunityId = Guid.NewGuid(),
            IsActive = true,
            DueDateFrom = DateTime.UtcNow,
            DueDateTo = DateTime.UtcNow.AddDays(7),
            Priority = "high"
        };

        // Assert
        query.Type.Should().Be("Call");
        query.Status.Should().Be("completed");
        query.AssignedUserId.Should().NotBeEmpty();
        query.CompanyId.Should().NotBeEmpty();
        query.ContactId.Should().NotBeEmpty();
        query.LeadId.Should().NotBeEmpty();
        query.OpportunityId.Should().NotBeEmpty();
        query.IsActive.Should().BeTrue();
        query.DueDateFrom.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        query.DueDateTo.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
        query.Priority.Should().Be("high");
    }

    [Fact]
    public void NoteFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new NoteFilterQuery
        {
            SearchTerm = "important",
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            LeadId = Guid.NewGuid(),
            OpportunityId = Guid.NewGuid(),
            CreatedBy = Guid.NewGuid(),
            IsActive = true,
            CreatedFrom = DateTime.UtcNow.AddDays(-10),
            CreatedTo = DateTime.UtcNow
        };

        // Assert
        query.SearchTerm.Should().Be("important");
        query.CompanyId.Should().NotBeEmpty();
        query.ContactId.Should().NotBeEmpty();
        query.LeadId.Should().NotBeEmpty();
        query.OpportunityId.Should().NotBeEmpty();
        query.CreatedBy.Should().NotBeEmpty();
        query.IsActive.Should().BeTrue();
        query.CreatedFrom.Should().BeCloseTo(DateTime.UtcNow.AddDays(-10), TimeSpan.FromSeconds(1));
        query.CreatedTo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void OpportunityFilterQuery_AllProperties_CanBeSet()
    {
        // Arrange & Act
        var query = new OpportunityFilterQuery
        {
            Title = "Big Deal",
            CompanyId = Guid.NewGuid(),
            ContactId = Guid.NewGuid(),
            Stage = "Proposal",
            AmountMin = 10000,
            AmountMax = 100000,
            ProbabilityMin = 50,
            ProbabilityMax = 100,
            ExpectedCloseDateFrom = DateTime.UtcNow,
            ExpectedCloseDateTo = DateTime.UtcNow.AddMonths(3),
            IsActive = true,
            AssignedUserId = Guid.NewGuid(),
            SearchTerm = "deal",
            CreatedFrom = DateTime.UtcNow.AddDays(-30),
            CreatedTo = DateTime.UtcNow
        };

        // Assert
        query.Title.Should().Be("Big Deal");
        query.CompanyId.Should().NotBeEmpty();
        query.ContactId.Should().NotBeEmpty();
        query.Stage.Should().Be("Proposal");
        query.AmountMin.Should().Be(10000);
        query.AmountMax.Should().Be(100000);
        query.ProbabilityMin.Should().Be(50);
        query.ProbabilityMax.Should().Be(100);
        query.ExpectedCloseDateFrom.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        query.ExpectedCloseDateTo.Should().BeCloseTo(DateTime.UtcNow.AddMonths(3), TimeSpan.FromSeconds(1));
        query.IsActive.Should().BeTrue();
        query.AssignedUserId.Should().NotBeEmpty();
        query.SearchTerm.Should().Be("deal");
        query.CreatedFrom.Should().BeCloseTo(DateTime.UtcNow.AddDays(-30), TimeSpan.FromSeconds(1));
        query.CreatedTo.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
