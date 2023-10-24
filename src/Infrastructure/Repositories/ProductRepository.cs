using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IRepositoryAsync<Product, int> _repository;

    public ProductRepository(IRepositoryAsync<Product, int> repository) => _repository = repository;

    public async Task<bool> IsBrandUsed(int brandId) => await _repository.Entities.AnyAsync(b => b.BrandId == brandId);
}
