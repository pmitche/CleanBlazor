using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Queries;

[ExcludeFromCodeCoverage]
public sealed record ExportProductsQuery(string SearchString = "") : IQuery<Result<string>>;

internal sealed class ExportProductsQueryHandler : IQueryHandler<ExportProductsQuery, Result<string>>
{
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;
    private readonly IProductRepository _productRepository;

    public ExportProductsQueryHandler(
        IExcelService excelService,
        IStringLocalizer<ExportProductsQueryHandler> localizer,
        IProductRepository productRepository)
    {
        _excelService = excelService;
        _localizer = localizer;
        _productRepository = productRepository;
    }

    public async Task<Result<string>> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        ProductFilterSpecification productFilterSpec = new(request.SearchString);
        var products = await _productRepository.Entities
            .Specify(productFilterSpec)
            .ToListAsync(cancellationToken);
        var data = await _excelService.ExportAsync(products,
            new Dictionary<string, Func<Product, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Barcode"], item => item.Barcode },
                { _localizer["Description"], item => item.Description },
                { _localizer["Rate"], item => item.Rate }
            },
            _localizer["Products"]);

        return data;
    }
}
