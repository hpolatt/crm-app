using Microsoft.EntityFrameworkCore.Storage;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;
using PktApp.Infrastructure.Data;

namespace PktApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new Repository<User>(context);
        Companies = new Repository<Company>(context);
        Contacts = new Repository<Contact>(context);
        Leads = new Repository<Lead>(context);
        Opportunities = new Repository<Opportunity>(context);
        Activities = new Repository<Activity>(context);
        DealStages = new Repository<DealStage>(context);
        Notes = new Repository<Note>(context);
        SystemSettings = new Repository<SystemSetting>(context);
        ActivityLogs = new Repository<ActivityLog>(context);
    }

    public IRepository<User> Users { get; }
    public IRepository<Company> Companies { get; }
    public IRepository<Contact> Contacts { get; }
    public IRepository<Lead> Leads { get; }
    public IRepository<Opportunity> Opportunities { get; }
    public IRepository<Activity> Activities { get; }
    public IRepository<DealStage> DealStages { get; }
    public IRepository<Note> Notes { get; }
    public IRepository<SystemSetting> SystemSettings { get; }
    public IRepository<ActivityLog> ActivityLogs { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
