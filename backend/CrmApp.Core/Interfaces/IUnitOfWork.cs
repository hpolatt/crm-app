using PKTApp.Domain.Entities;

namespace PKTApp.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<DelayReason> DelayReasons { get; }
    IRepository<Reactor> Reactors { get; }
    IRepository<Product> Products { get; }
    IRepository<PktTransaction> PktTransactions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
