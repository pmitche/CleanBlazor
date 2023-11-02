using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class ProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsBrandUsedAsync(int brandId, CancellationToken cancellationToken = default) =>
        await Entities.AnyAsync(b => b.BrandId == brandId, cancellationToken);
}
