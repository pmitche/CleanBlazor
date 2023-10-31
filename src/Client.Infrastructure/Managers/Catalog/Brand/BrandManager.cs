﻿using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Brand;

public class BrandManager : IBrandManager
{
    private readonly HttpClient _httpClient;

    public BrandManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<string>> ExportToExcelAsync(string searchString = "")
    {
        HttpResponseMessage response = await _httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? BrandsEndpoints.Export
            : BrandsEndpoints.ExportFiltered(searchString));
        return await response.ToResult<string>();
    }

    public async Task<Result<int>> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync(BrandsEndpoints.DeleteById(id));
        return await response.ToResult<int>();
    }

    public async Task<Result<List<GetAllBrandsResponse>>> GetAllAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(BrandsEndpoints.GetAll);
        return await response.ToResult<List<GetAllBrandsResponse>>();
    }

    public async Task<Result<int>> SaveAsync(AddEditBrandRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Save, request);
        return await response.ToResult<int>();
    }

    public async Task<Result<int>> ImportAsync(UploadRequest request)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(BrandsEndpoints.Import, request);
        return await response.ToResult<int>();
    }
}
