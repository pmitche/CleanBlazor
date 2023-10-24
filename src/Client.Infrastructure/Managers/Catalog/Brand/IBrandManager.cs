using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;

public interface IBrandManager : IManager
{
    Task<IResult<List<GetAllBrandsResponse>>> GetAllAsync();

    Task<IResult<int>> SaveAsync(AddEditBrandCommand request);

    Task<IResult<int>> DeleteAsync(int id);

    Task<IResult<string>> ExportToExcelAsync(string searchString = "");

    Task<IResult<int>> ImportAsync(ImportBrandsCommand request);
}
