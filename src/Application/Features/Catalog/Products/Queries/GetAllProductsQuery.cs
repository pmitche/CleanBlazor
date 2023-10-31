using System.Diagnostics.CodeAnalysis;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Products;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAllProductsQuery(int PageNumber, int PageSize, string SearchString, string OrderByInput)
    : IQuery<PaginatedResult<GetAllPagedProductsResponse>>
{
    public string[] OrderBy =>
        string.IsNullOrWhiteSpace(OrderByInput) ? Array.Empty<string>() : OrderByInput.Split(',');
}

internal sealed class GetAllProductsQueryHandler
    : IQueryHandler<GetAllProductsQuery, PaginatedResult<GetAllPagedProductsResponse>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedResult<GetAllPagedProductsResponse>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Product, GetAllPagedProductsResponse>> expression = e => new GetAllPagedProductsResponse
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            Rate = e.Rate,
            Barcode = e.Barcode,
            Brand = e.Brand.Name,
            BrandId = e.BrandId
        };
        ProductFilterSpecification productFilterSpec = new(request.SearchString);
        var queryable = _productRepository.Entities
            .Specify(productFilterSpec);

        if (!string.IsNullOrWhiteSpace(request.OrderByInput))
        {
            queryable = queryable.OrderBy(request.OrderByInput);
        }

        return await queryable
            .Select(expression)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
