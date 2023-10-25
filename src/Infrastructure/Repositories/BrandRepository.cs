using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class BrandRepository : GenericRepository<Brand, int>, IBrandRepository
{
    public BrandRepository(BlazorHeroContext dbContext) : base(dbContext)
    {
    }
}
