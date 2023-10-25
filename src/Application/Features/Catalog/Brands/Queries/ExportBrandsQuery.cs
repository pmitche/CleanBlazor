using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Catalog;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Queries;

[ExcludeFromCodeCoverage]
public sealed record ExportBrandsQuery(string SearchString = "") : IQuery<Result<string>>;

internal sealed class ExportBrandsQueryHandler : IQueryHandler<ExportBrandsQuery, Result<string>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportBrandsQueryHandler> _localizer;

    public ExportBrandsQueryHandler(
        IExcelService excelService,
        IStringLocalizer<ExportBrandsQueryHandler> localizer,
        IBrandRepository brandRepository)
    {
        _excelService = excelService;
        _localizer = localizer;
        _brandRepository = brandRepository;
    }

    public async Task<Result<string>> Handle(ExportBrandsQuery request, CancellationToken cancellationToken)
    {
        BrandFilterSpecification brandFilterSpec = new(request.SearchString);
        var brands = await _brandRepository.Entities
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
