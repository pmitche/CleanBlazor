using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Application.Features.Products.Commands;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Product;

public class ProductManager : IProductManager
{
    private readonly HttpClient _httpClient;

    public ProductManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<IResult<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"{ProductsEndpoints.Delete}/{id}");
        return await response.ToResult<int>();
    }

    public async Task<IResult<string>> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? ProductsEndpoints.Export
            : ProductsEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<IResult<string>> GetProductImageAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ProductsEndpoints.GetProductImage(id));
        return await response.ToResult<string>();
    }

    public async Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ProductsEndpoints.GetAllPaged(request.PageNumber,
            request.PageSize,
            request.SearchString,
            request.OrderBy));
        return await response.ToPaginatedResult<GetAllPagedProductsResponse>();
    }

    public async Task<IResult<int>> SaveAsync(AddEditProductCommand request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ProductsEndpoints.Save, request);
        return await response.ToResult<int>();
    }
}
