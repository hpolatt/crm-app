using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using PktApp.Infrastructure.Data;
using PktApp.Domain.Entities;

namespace PktApp.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string DbName = $"TestDb_{Guid.NewGuid()}";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Add test-specific configuration
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "TestSecretKeyForIntegrationTesting1234567890",
                ["JwtSettings:Issuer"] = "CrmApp",
                ["JwtSettings:Audience"] = "CrmAppUsers",
                ["JwtSettings:ExpiryMinutes"] = "60"
            });
        });
        
        builder.ConfigureServices(services =>
        {
            // Remove the real DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing - use same DB name across all requests
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(DbName);
            });

            // Ensure database is created and seed base roles
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();
            
            // Seed default roles if they don't exist
            SeedDefaultRoles(db);
        });

        builder.UseEnvironment("Testing");
    }
    
    private void SeedDefaultRoles(ApplicationDbContext context)
    {
        // Only seed if roles don't exist
        if (!context.Roles.Any())
        {
            var roles = new[]
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    Description = "Default user role",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrator role",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Manager",
                    Description = "Manager role",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
            
            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }
}
