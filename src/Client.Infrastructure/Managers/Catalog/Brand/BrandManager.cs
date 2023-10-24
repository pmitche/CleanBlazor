using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;

public class BrandManager : IBrandManager
{
    private readonly HttpClient _httpClient;

    public BrandManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? BrandsEndpoints.Export
            : BrandsEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<IResult<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{BrandsEndpoints.Delete}/{id}");
        return await response.ToResult<int>();
    }

    public async Task<IResult<List<GetAllBrandsResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(BrandsEndpoints.GetAll);
        return await response.ToResult<List<GetAllBrandsResponse>>();
    }

    public async Task<IResult<int>> SaveAsync(AddEditBrandCommand request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Save, request);
        return await response.ToResult<int>();
    }

    public async Task<IResult<int>> ImportAsync(ImportBrandsCommand request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Import, request);
        return await response.ToResult<int>();
    }
}
