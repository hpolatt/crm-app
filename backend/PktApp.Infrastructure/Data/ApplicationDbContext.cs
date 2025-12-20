using Microsoft.EntityFrameworkCore;
using PktApp.Domain.Entities;
using PktApp.Domain.Enums;

namespace PktApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // PKT Schema
    public DbSet<DelayReason> DelayReasons { get; set; } = null!;
    public DbSet<Reactor> Reactors { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<PktTransaction> PktTransactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema
        modelBuilder.HasDefaultSchema("public");

        // Configure all DateTime properties to use UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(v, DateTimeKind.Utc) : v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        )
                    );
                }
            }
        }

        // Configure PKT entities
        modelBuilder.Entity<DelayReason>(entity =>
        {
            entity.ToTable("DelayReasons");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Reactor>(entity =>
        {
            entity.ToTable("Reactors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SBU).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<PktTransaction>(entity =>
        {
            entity.ToTable("PktTransactions");
            entity.HasKey(e => e.Id);
            
            // PostgreSQL enum mapping - Npgsql handles the enum conversion automatically
            entity.Property(e => e.Status)
                .HasColumnName("Status");
            
            entity.HasOne(e => e.Reactor)
                .WithMany(r => r.PktTransactions)
                .HasForeignKey(e => e.ReactorId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.PktTransactions)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.DelayReason)
                .WithMany(d => d.PktTransactions)
                .HasForeignKey(e => e.DelayReasonId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Configure column names to match database PascalCase
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name);
            }
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (BaseEntity)entry.Entity;
            
            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
            }
            
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
