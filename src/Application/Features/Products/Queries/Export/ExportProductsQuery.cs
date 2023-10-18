using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Queries.Export;

public record ExportProductsQuery(string SearchString = "") : IRequest<Result<string>>;

internal class ExportProductsQueryHandler : IRequestHandler<ExportProductsQuery, Result<string>>
{
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportProductsQueryHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public ExportProductsQueryHandler(
        IExcelService excelService,
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<ExportProductsQueryHandler> localizer)
    {
        _excelService = excelService;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        ProductFilterSpecification productFilterSpec = new(request.SearchString);
        List<Product> products = await _unitOfWork.Repository<Product>().Entities
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

        return await Result<string>.SuccessAsync(data: data);
    }
}
