using CrmApp.Domain.Entities;

namespace CrmApp.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Company> Companies { get; }
    IRepository<Contact> Contacts { get; }
    IRepository<Lead> Leads { get; }
    IRepository<Opportunity> Opportunities { get; }
    IRepository<Activity> Activities { get; }
    IRepository<DealStage> DealStages { get; }
    IRepository<Note> Notes { get; }
    IRepository<SystemSetting> SystemSettings { get; }
    IRepository<ActivityLog> ActivityLogs { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
