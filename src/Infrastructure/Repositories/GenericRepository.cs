using BlazorHero.CleanArchitecture.Domain.Abstractions;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal abstract class GenericRepository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : class, IEntity<TId>
{
    protected GenericRepository(BlazorHeroContext dbContext) => DbContext = dbContext;
    protected BlazorHeroContext DbContext { get; }

    public virtual IQueryable<TEntity> Entities => DbContext.Set<TEntity>();

    public async Task<TEntity> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await Entities.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await Entities.ToListAsync(cancellationToken);

    public async Task<List<TEntity>> GetPagedResponseAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default) =>
        await Entities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public virtual TEntity Add(TEntity entity) => DbContext.Set<TEntity>().Add(entity).Entity;

    public virtual void AddRange(IEnumerable<TEntity> entities) => DbContext.Set<TEntity>().AddRange(entities);

    public virtual void Update(TEntity entity) => DbContext.Set<TEntity>().Update(entity);

    public virtual TEntity Remove(TEntity entity) => DbContext.Set<TEntity>().Remove(entity).Entity;
}
