using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.AddEdit;

public class AddEditBrandCommand : IRequest<Result<int>>
{
    public int Id { get; set; }

    [Required] public string Name { get; set; }

    [Required] public string Description { get; set; }

    [Required] public decimal Tax { get; set; }
}

internal class AddEditBrandCommandHandler : IRequestHandler<AddEditBrandCommand, Result<int>>
{
    private readonly IStringLocalizer<AddEditBrandCommandHandler> _localizer;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public AddEditBrandCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IMapper mapper,
        IStringLocalizer<AddEditBrandCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(AddEditBrandCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == 0)
        {
            var brand = _mapper.Map<Brand>(command);
            await _unitOfWork.Repository<Brand>().AddAsync(brand);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
            return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Saved"]);
        }
        else
        {
            Brand brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(command.Id);
            if (brand == null)
            {
                return await Result<int>.FailAsync(_localizer["Brand Not Found!"]);
            }

            brand.Name = command.Name ?? brand.Name;
            brand.Tax = command.Tax == 0 ? brand.Tax : command.Tax;
            brand.Description = command.Description ?? brand.Description;
            await _unitOfWork.Repository<Brand>().UpdateAsync(brand);
            await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                ApplicationConstants.Cache.GetAllBrandsCacheKey);
            return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Updated"]);
        }
    }
}
