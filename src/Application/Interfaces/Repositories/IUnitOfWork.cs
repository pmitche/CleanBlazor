using BlazorHero.CleanArchitecture.Domain.Contracts;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;

public interface IUnitOfWork<TId> : IDisposable
{
    IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntity<TId>;

    Task<int> Commit(CancellationToken cancellationToken);

    Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

    Task Rollback();
}
