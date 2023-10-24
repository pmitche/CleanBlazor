﻿using BlazorHero.CleanArchitecture.Application.Features.Products.Commands;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Catalog.Product;

public interface IProductManager : IManager
{
    Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request);

    Task<IResult<string>> GetProductImageAsync(int id);

    Task<IResult<int>> SaveAsync(AddEditProductCommand request);

    Task<IResult<int>> DeleteAsync(int id);

    Task<IResult<string>> ExportToExcelAsync(string searchString = "");
}
