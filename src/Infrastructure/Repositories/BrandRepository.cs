using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Data;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class BrandRepository : GenericRepository<Brand, int>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
