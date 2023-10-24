namespace BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;

public interface IProductRepository
{
    Task<bool> IsBrandUsed(int brandId);
}
