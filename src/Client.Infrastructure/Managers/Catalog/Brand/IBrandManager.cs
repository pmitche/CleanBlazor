using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;

public interface IBrandManager : IManager
{
    Task<IResult<List<GetAllBrandsResponse>>> GetAllAsync();

    Task<IResult<int>> SaveAsync(AddEditBrandRequest request);

    Task<IResult<int>> DeleteAsync(int id);

    Task<IResult<string>> ExportToExcelAsync(string searchString = "");

    Task<IResult<int>> ImportAsync(UploadRequest request);
}
