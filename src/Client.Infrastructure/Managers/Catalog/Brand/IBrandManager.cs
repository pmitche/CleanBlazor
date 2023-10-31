using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;

public interface IBrandManager : IManager
{
    Task<Result<List<GetAllBrandsResponse>>> GetAllAsync();

    Task<Result<int>> SaveAsync(AddEditBrandRequest request);

    Task<Result<int>> DeleteAsync(int id);

    Task<Result<string>> ExportToExcelAsync(string searchString = "");

    Task<Result<int>> ImportAsync(UploadRequest request);
}
