using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Interfaces.Messaging;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.Export;

public record ExportBrandsQuery(string SearchString = "") : IQuery<Result<string>>;

internal class ExportBrandsQueryHandler : IQueryHandler<ExportBrandsQuery, Result<string>>
{
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportBrandsQueryHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public ExportBrandsQueryHandler(
        IExcelService excelService,
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<ExportBrandsQueryHandler> localizer)
    {
        _excelService = excelService;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(ExportBrandsQuery request, CancellationToken cancellationToken)
    {
        BrandFilterSpecification brandFilterSpec = new(request.SearchString);
        List<Brand> brands = await _unitOfWork.Repository<Brand>().Entities
            .Specify(brandFilterSpec)
            .ToListAsync(cancellationToken);
        var data = await _excelService.ExportAsync(brands,
            new Dictionary<string, Func<Brand, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Description"], item => item.Description },
                { _localizer["Tax"], item => item.Tax }
            },
            _localizer["Brands"]);

        return await Result<string>.SuccessAsync(data: data);
    }
}
