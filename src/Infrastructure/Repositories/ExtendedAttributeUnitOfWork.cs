using System.Collections;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using LazyCache;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

public class
    ExtendedAttributeUnitOfWork<TId, TEntityId, TEntity> : IExtendedAttributeUnitOfWork<TId, TEntityId, TEntity>
    where TEntity : AuditableEntity<TEntityId>
{
    private readonly IAppCache _cache;
    private readonly ICurrentUserService _currentUserService;
    private readonly BlazorHeroContext _dbContext;
    private bool _disposed;
    private Hashtable _repositories;

    public ExtendedAttributeUnitOfWork(
        BlazorHeroContext dbContext,
        ICurrentUserService currentUserService,
        IAppCache cache)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _currentUserService = currentUserService;
        _cache = cache;
    }

    public IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            Type repositoryType = typeof(RepositoryAsync<,>);

            var repositoryInstance =
                Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T), typeof(TId)), _dbContext);

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepositoryAsync<T, TId>)_repositories[type];
    }

    public async Task<int> Commit(CancellationToken cancellationToken) =>
        await _dbContext.SaveChangesAsync(cancellationToken);

    public async Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys)
    {
        var result = await _dbContext.SaveChangesAsync(cancellationToken);
        foreach (var cacheKey in cacheKeys)
        {
            _cache.Remove(cacheKey);
        }

        return result;
    }

    public Task Rollback()
    {
        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                //dispose managed resources
                _dbContext.Dispose();
            }
        }

        //dispose unmanaged resources
        _disposed = true;
    }
}
