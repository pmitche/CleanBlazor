using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Contracts;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    protected GenericRepository(BlazorHeroContext dbContext) => DbContext = dbContext;
    protected BlazorHeroContext DbContext { get; }

    public IQueryable<TEntity> Entities => DbContext.Set<TEntity>();

    public async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await DbContext.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken) =>
        await DbContext.Set<TEntity>().ToListAsync(cancellationToken);

    public async Task<List<TEntity>> GetPagedResponseAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        await DbContext
            .Set<TEntity>()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public TEntity Add(TEntity entity) => DbContext.Set<TEntity>().Add(entity).Entity;

    public void AddRange(IReadOnlyCollection<TEntity> entities) => DbContext.Set<TEntity>().AddRange(entities);

    public void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);

    public TEntity Remove(TEntity entity) => DbContext.Set<TEntity>().Remove(entity).Entity;
}
