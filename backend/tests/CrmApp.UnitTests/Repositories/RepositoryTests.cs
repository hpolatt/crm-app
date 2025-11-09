using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;
using CrmApp.Infrastructure.Data;
using CrmApp.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrmApp.UnitTests.Repositories;

public class RepositoryTests
{
    private ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        return context;
    }

    [Fact]
    public async Task AddAsync_WithValidEntity_AddsEntityToDatabase()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var company = new Company
        {
            Name = "Test Company",
            Industry = "Technology"
        };

        // Act
        var result = await repository.AddAsync(company);
        await context.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        
        var savedCompany = await context.Companies.FindAsync(result.Id);
        savedCompany.Should().NotBeNull();
        savedCompany!.Name.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Test Company",
            Industry = "Technology",
            CreatedAt = DateTime.UtcNow
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(company.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(company.Id);
        result.Name.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithDeletedEntity_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Deleted Company",
            Industry = "Tech",
            IsDeleted = true,
            CreatedAt = DateTime.UtcNow
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetByIdAsync(company.Id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllNonDeletedEntities()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Company 1", Industry = "Tech", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Company 2", Industry = "Finance", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Deleted Company", Industry = "Retail", IsDeleted = true, CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => !c.IsDeleted);
        result.Select(c => c.Name).Should().Contain(new[] { "Company 1", "Company 2" });
    }

    [Fact]
    public async Task FindAsync_WithPredicate_ReturnsMatchingEntities()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Tech Corp", Industry = "Technology", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Tech Solutions", Industry = "Technology", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Finance Inc", Industry = "Finance", CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FindAsync(c => c.Industry == "Technology");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(c => c.Industry == "Technology");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithMatchingPredicate_ReturnsFirstEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "First Tech", Industry = "Technology", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Second Tech", Industry = "Technology", CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        var result = await repository.FirstOrDefaultAsync(c => c.Industry == "Technology");

        // Assert
        result.Should().NotBeNull();
        result!.Industry.Should().Be("Technology");
    }

    [Fact]
    public async Task FirstOrDefaultAsync_WithNoMatch_ReturnsNull()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);

        // Act
        var result = await repository.FirstOrDefaultAsync(c => c.Industry == "NonExistent");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_WithValidEntity_UpdatesEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Industry = "Tech",
            CreatedAt = DateTime.UtcNow
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act
        company.Name = "Updated Name";
        repository.Update(company);
        await context.SaveChangesAsync();

        // Assert
        var updatedCompany = await context.Companies.FindAsync(company.Id);
        updatedCompany.Should().NotBeNull();
        updatedCompany!.Name.Should().Be("Updated Name");
        updatedCompany.UpdatedAt.Should().NotBeNull();
        updatedCompany.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Remove_WithValidEntity_SoftDeletesEntity()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = "Company To Delete",
            Industry = "Tech",
            CreatedAt = DateTime.UtcNow
        };
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // Act
        repository.Remove(company);
        await context.SaveChangesAsync();

        // Assert
        var deletedCompany = await context.Companies.FindAsync(company.Id);
        deletedCompany.Should().NotBeNull();
        deletedCompany!.IsDeleted.Should().BeTrue();
        deletedCompany.UpdatedAt.Should().NotBeNull();
        
        // Verify it's not returned by GetByIdAsync
        var result = await repository.GetByIdAsync(company.Id);
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddRangeAsync_WithMultipleEntities_AddsAllEntities()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var companies = new[]
        {
            new Company { Name = "Company 1", Industry = "Tech" },
            new Company { Name = "Company 2", Industry = "Finance" },
            new Company { Name = "Company 3", Industry = "Retail" }
        };

        // Act
        await repository.AddRangeAsync(companies);
        await context.SaveChangesAsync();

        // Assert
        var allCompanies = await repository.GetAllAsync();
        allCompanies.Should().HaveCount(3);
        companies.Should().OnlyContain(c => c.Id != Guid.Empty);
        companies.Should().OnlyContain(c => c.CreatedAt != default);
    }

    [Fact]
    public async Task RemoveRange_WithMultipleEntities_SoftDeletesAllEntities()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Company 1", Industry = "Tech", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Company 2", Industry = "Finance", CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        repository.RemoveRange(companies);
        await context.SaveChangesAsync();

        // Assert
        var allCompanies = await repository.GetAllAsync();
        allCompanies.Should().BeEmpty();
        
        var deletedCompanies = await context.Companies.Where(c => c.IsDeleted).ToListAsync();
        deletedCompanies.Should().HaveCount(2);
    }

    [Fact]
    public async Task CountAsync_WithoutPredicate_ReturnsAllNonDeletedCount()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Company 1", Industry = "Tech", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Company 2", Industry = "Finance", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Deleted", Industry = "Retail", IsDeleted = true, CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        var count = await repository.CountAsync();

        // Assert
        count.Should().Be(2);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ReturnsMatchingCount()
    {
        // Arrange
        using var context = GetInMemoryDbContext();
        var repository = new Repository<Company>(context);
        var companies = new[]
        {
            new Company { Id = Guid.NewGuid(), Name = "Tech 1", Industry = "Technology", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Tech 2", Industry = "Technology", CreatedAt = DateTime.UtcNow },
            new Company { Id = Guid.NewGuid(), Name = "Finance Co", Industry = "Finance", CreatedAt = DateTime.UtcNow }
        };
        context.Companies.AddRange(companies);
        await context.SaveChangesAsync();

        // Act
        var count = await repository.CountAsync(c => c.Industry == "Technology");

        // Assert
        count.Should().Be(2);
    }
}
