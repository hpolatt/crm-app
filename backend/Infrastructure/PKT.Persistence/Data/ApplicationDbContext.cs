using Microsoft.EntityFrameworkCore;
using PKT.Domain.Entities;

namespace PKT.Persistence.Data;

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

        // Configure DelayReasons
        modelBuilder.Entity<DelayReason>(entity =>
        {
            entity.ToTable("DelayReasons");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure Reactors
        modelBuilder.Entity<Reactor>(entity =>
        {
            entity.ToTable("Reactors");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure Products
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SBU).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductCode).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.MinProductionQuantity).HasPrecision(18, 2);
            entity.Property(e => e.MaxProductionQuantity).HasPrecision(18, 2);
            entity.Property(e => e.ProductionDurationMinutes).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });

        // Configure PktTransactions
        modelBuilder.Entity<PktTransaction>(entity =>
        {
            entity.ToTable("PktTransactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.WorkOrderNo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LotNo).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CausticAmountKg).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).IsRequired();
            
            entity.HasIndex(e => e.ReactorId).HasDatabaseName("IX_PktTransactions_ReaktorId");
            entity.HasIndex(e => e.ProductId).HasDatabaseName("IX_PktTransactions_ProductId");
            entity.HasIndex(e => e.DelayReasonId).HasDatabaseName("IX_PktTransactions_DelayReasonId");
            
            // Configure relationships
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
