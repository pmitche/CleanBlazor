using System.Data;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.AddEdit;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Application.Interfaces.Services;
using BlazorHero.CleanArchitecture.Application.Requests;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.Import;

public class ImportBrandsCommand : IRequest<Result<int>>
{
    public UploadRequest UploadRequest { get; set; }
}

internal class ImportBrandsCommandHandler : IRequestHandler<ImportBrandsCommand, Result<int>>
{
    private readonly IValidator<AddEditBrandCommand> _addBrandValidator;
    private readonly IExcelService _excelService;
    private readonly IStringLocalizer<ImportBrandsCommandHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public ImportBrandsCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IExcelService excelService,
        IMapper mapper,
        IValidator<AddEditBrandCommand> addBrandValidator,
        IStringLocalizer<ImportBrandsCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _excelService = excelService;
        _mapper = mapper;
        _addBrandValidator = addBrandValidator;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(ImportBrandsCommand request, CancellationToken cancellationToken)
    {
        MemoryStream stream = new(request.UploadRequest.Data);
        IResult<IEnumerable<Brand>> result = await _excelService.ImportAsync(stream,
            new Dictionary<string, Func<DataRow, Brand, object>>
            {
                { _localizer["Name"], (row, item) => item.Name = row[_localizer["Name"]].ToString() },
                {
                    _localizer["Description"],
                    (row, item) => item.Description = row[_localizer["Description"]].ToString()
                },
                {
                    _localizer["Tax"], (row, item) =>
                        item.Tax = decimal.TryParse(row[_localizer["Tax"]].ToString(), out var tax) ? tax : 1
                }
            },
            _localizer["Brands"]);

        if (!result.Succeeded)
        {
            return await Result<int>.FailAsync(result.Messages);
        }

        IEnumerable<Brand> importedBrands = result.Data;
        List<string> errors = new();
        var errorsOccurred = false;
        foreach (Brand brand in importedBrands)
        {
            ValidationResult validationResult =
                await _addBrandValidator.ValidateAsync(_mapper.Map<AddEditBrandCommand>(brand), cancellationToken);
            if (validationResult.IsValid)
            {
                await _unitOfWork.Repository<Brand>().AddAsync(brand);
            }
            else
            {
                errorsOccurred = true;
                errors.AddRange(validationResult.Errors.Select(e =>
                    $"{(!string.IsNullOrWhiteSpace(brand.Name) ? $"{brand.Name} - " : string.Empty)}{e.ErrorMessage}"));
            }
        }

        if (errorsOccurred)
        {
            return await Result<int>.FailAsync(errors);
        }

        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
        return await Result<int>.SuccessAsync(result.Data.Count(), result.Messages[0]);

    }
}
