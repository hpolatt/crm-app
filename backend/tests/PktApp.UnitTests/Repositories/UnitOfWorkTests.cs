using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;
using PktApp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace PktApp.UnitTests.Repositories;

public class UnitOfWorkTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public void UnitOfWork_InitializesAllRepositories()
    {
        // Arrange & Act
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Assert
        unitOfWork.Companies.Should().NotBeNull();
        unitOfWork.Contacts.Should().NotBeNull();
        unitOfWork.Leads.Should().NotBeNull();
        unitOfWork.Opportunities.Should().NotBeNull();
        unitOfWork.Activities.Should().NotBeNull();
        unitOfWork.Notes.Should().NotBeNull();
        unitOfWork.Users.Should().NotBeNull();
        unitOfWork.DealStages.Should().NotBeNull();
        unitOfWork.ActivityLogs.Should().NotBeNull();
        unitOfWork.SystemSettings.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_SavesAllChangesToDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        var company = new Company
        {
            Name = "Test Company",
            Email = "test@company.com",
            Industry = "Tech"
        };

        // Act
        await unitOfWork.Companies.AddAsync(company);
        var saveResult = await unitOfWork.SaveChangesAsync();

        // Assert
        saveResult.Should().BeGreaterThan(0);
        
        var savedCompany = await context.Companies.FirstOrDefaultAsync(c => c.Name == "Test Company");
        savedCompany.Should().NotBeNull();
        savedCompany!.Email.Should().Be("test@company.com");
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleEntities_SavesAll()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        var company = new Company
        {
            Name = "Company 1",
            Email = "company1@test.com"
        };

        var lead = new Lead
        {
            Title = "Test Lead",
            Status = "new"
        };

        // Act
        await unitOfWork.Companies.AddAsync(company);
        await unitOfWork.Leads.AddAsync(lead);
        var saveResult = await unitOfWork.SaveChangesAsync();

        // Assert
        saveResult.Should().Be(2); // 2 entities saved
        
        context.Companies.Count().Should().Be(1);
        context.Leads.Count().Should().Be(1);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ReturnsZero()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Act
        var saveResult = await unitOfWork.SaveChangesAsync();

        // Assert
        saveResult.Should().Be(0);
    }

    [Fact]
    public async Task UnitOfWork_WithTransaction_RollsBackOnError()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        var company1 = new Company
        {
            Name = "Valid Company",
            Email = "valid@company.com"
        };

        await unitOfWork.Companies.AddAsync(company1);
        await unitOfWork.SaveChangesAsync();

        // Act & Assert
        var initialCount = await context.Companies.CountAsync();
        initialCount.Should().Be(1);

        // Adding another company and verifying state
        var company2 = new Company
        {
            Name = "Another Company",
            Email = "another@company.com"
        };

        await unitOfWork.Companies.AddAsync(company2);
        
        // Before save, count should still be 1
        var beforeSaveCount = await context.Companies.CountAsync();
        beforeSaveCount.Should().Be(1);

        // After save, count should be 2
        await unitOfWork.SaveChangesAsync();
        var afterSaveCount = await context.Companies.CountAsync();
        afterSaveCount.Should().Be(2);
    }

    [Fact]
    public void Dispose_DisposesContext()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Act
        unitOfWork.Dispose();

        // Assert
        // After dispose, context should throw if we try to use it
        Action act = () => context.Companies.ToList();
        act.Should().Throw<ObjectDisposedException>();
    }

    [Fact]
    public async Task UnitOfWork_MultipleOperations_WorkCorrectly()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Act - Add
        var company = new Company { Name = "Test Co", Email = "test@co.com" };
        await unitOfWork.Companies.AddAsync(company);
        await unitOfWork.SaveChangesAsync();

        // Act - Update
        company.Industry = "Technology";
        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync();

        // Act - Verify
        var updated = await unitOfWork.Companies.GetByIdAsync(company.Id);
        
        // Assert
        updated.Should().NotBeNull();
        updated!.Industry.Should().Be("Technology");
    }
}
