namespace BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<bool> IsBrandUsed(int brandId);
}
