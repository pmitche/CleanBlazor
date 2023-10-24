using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Contracts.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetAllPaged;

public record GetAllProductsQuery(int PageNumber, int PageSize, string SearchString, string OrderByInput)
    : IQuery<PaginatedResult<GetAllPagedProductsResponse>>
{
    public string[] OrderBy =>
        string.IsNullOrWhiteSpace(OrderByInput) ? Array.Empty<string>() : OrderByInput.Split(',');
}

internal class GetAllProductsQueryHandler
    : IQueryHandler<GetAllProductsQuery, PaginatedResult<GetAllPagedProductsResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

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
        if (request.OrderBy?.Any() != true)
        {
            PaginatedResult<GetAllPagedProductsResponse> data = await _unitOfWork.Repository<Product>().Entities
                .Specify(productFilterSpec)
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return data;
        }
        else
        {
            var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
            PaginatedResult<GetAllPagedProductsResponse> data = await _unitOfWork.Repository<Product>().Entities
                .Specify(productFilterSpec)
                .OrderBy(ordering) // require system.linq.dynamic.core
                .Select(expression)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
            return data;
        }
    }
}
