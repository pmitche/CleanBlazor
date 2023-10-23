using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Interfaces.Messaging;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Specifications.Misc;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.Export;

public record ExportDocumentTypesQuery(string SearchString = "") : IQuery<Result<string>>;

internal class ExportDocumentTypesQueryHandler : IQueryHandler<ExportDocumentTypesQuery, Result<string>>
{
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportDocumentTypesQueryHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public ExportDocumentTypesQueryHandler(
        IExcelService excelService,
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<ExportDocumentTypesQueryHandler> localizer)
    {
        _excelService = excelService;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<string>> Handle(ExportDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        DocumentTypeFilterSpecification documentTypeFilterSpec = new(request.SearchString);
        List<DocumentType> documentTypes = await _unitOfWork.Repository<DocumentType>().Entities
            .Specify(documentTypeFilterSpec)
            .ToListAsync(cancellationToken);
        var data = await _excelService.ExportAsync(documentTypes,
            new Dictionary<string, Func<DocumentType, object>>
            {
                { _localizer["Id"], item => item.Id },
                { _localizer["Name"], item => item.Name },
                { _localizer["Description"], item => item.Description }
            },
            _localizer["Document Types"]);

        return await Result<string>.SuccessAsync(data: data);
    }
}
