using CrmApp.Domain.Entities;

namespace CrmApp.UnitTests.Helpers;

/// <summary>
/// Factory for creating test data entities with valid default values
/// </summary>
public static class TestDataFactory
{
    public static User CreateUser(
        string? email = null,
        string? firstName = null,
        string? lastName = null,
        bool isActive = true)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email ?? $"test.user.{Guid.NewGuid():N}@test.com",
            FirstName = firstName ?? "Test",
            LastName = lastName ?? "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!"),
            IsActive = isActive,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>()
        };
    }

    public static Role CreateRole(string? name = null)
    {
        return new Role
        {
            Id = Guid.NewGuid(),
            Name = name ?? "User",
            Description = $"Test role: {name ?? "User"}",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Company CreateCompany(string? name = null)
    {
        return new Company
        {
            Id = Guid.NewGuid(),
            Name = name ?? $"Test Company {Guid.NewGuid():N}",
            Email = $"company.{Guid.NewGuid():N}@test.com",
            Phone = "+1-555-0100",
            Website = "https://testcompany.com",
            Industry = "Technology",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Contact CreateContact(Guid? companyId = null)
    {
        return new Contact
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            FirstName = "John",
            LastName = "Doe",
            Email = $"contact.{Guid.NewGuid():N}@test.com",
            Phone = "+1-555-0101",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Note CreateNote(
        Guid? companyId = null,
        Guid? contactId = null,
        Guid? leadId = null,
        Guid? opportunityId = null,
        string? content = null)
    {
        return new Note
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            ContactId = contactId,
            LeadId = leadId,
            OpportunityId = opportunityId,
            Content = content ?? "Test note content",
            IsPinned = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static RefreshToken CreateRefreshToken(Guid userId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };
    }
}
