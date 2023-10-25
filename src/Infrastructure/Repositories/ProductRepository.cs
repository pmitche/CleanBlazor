using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public ProductRepository(BlazorHeroContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsBrandUsedAsync(int brandId, CancellationToken cancellationToken = default) =>
        await Entities.AnyAsync(b => b.BrandId == brandId, cancellationToken);
}
