using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;

namespace BlazorHero.CleanArchitecture.Domain.Repositories;

public interface IProductRepository : IRepository<Product, int>
{
    Task<bool> IsBrandUsedAsync(int brandId, CancellationToken cancellationToken = default);
}
