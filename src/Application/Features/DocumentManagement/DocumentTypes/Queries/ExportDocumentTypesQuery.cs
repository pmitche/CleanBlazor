using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Misc;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Queries;

[ExcludeFromCodeCoverage]
public sealed record ExportDocumentTypesQuery(string SearchString = "") : IQuery<Result<string>>;

internal sealed class ExportDocumentTypesQueryHandler : IQueryHandler<ExportDocumentTypesQuery, Result<string>>
{
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ExportDocumentTypesQueryHandler> _localizer;
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public ExportDocumentTypesQueryHandler(
        IExcelService excelService,
        IStringLocalizer<ExportDocumentTypesQueryHandler> localizer,
        IDocumentTypeRepository documentTypeRepository)
    {
        _excelService = excelService;
        _localizer = localizer;
        _documentTypeRepository = documentTypeRepository;
    }

    public async Task<Result<string>> Handle(ExportDocumentTypesQuery request, CancellationToken cancellationToken)
    {
        DocumentTypeFilterSpecification documentTypeFilterSpec = new(request.SearchString);
        var documentTypes = await _documentTypeRepository.Entities
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

        return data;
    }
}
