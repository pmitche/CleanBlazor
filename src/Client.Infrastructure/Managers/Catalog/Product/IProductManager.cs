using BlazorHero.CleanArchitecture.Contracts.Catalog.Products;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Product;

public interface IProductManager : IManager
{
    Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

    Task<Result<string>> GetProductImageAsync(int id);

    Task<Result<int>> SaveAsync(AddEditProductRequest request);

    Task<Result<int>> DeleteAsync(int id);

    Task<Result<string>> ExportToExcelAsync(string searchString = "");
}
