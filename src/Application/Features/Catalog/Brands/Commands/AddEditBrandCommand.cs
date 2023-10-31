using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Commands;

[ExcludeFromCodeCoverage]
public sealed class AddEditBrandCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Tax { get; set; }

    public AddEditBrandCommand(int id, string name, string description, decimal tax)
    {
        Id = id;
        Name = name;
        Description = description;
        Tax = tax;
    }
}

internal sealed class AddEditBrandCommandHandler : ICommandHandler<AddEditBrandCommand, Result<int>>
{
    private readonly IStringLocalizer<AddEditBrandCommandHandler> _localizer;
    private readonly IBrandRepository _brandRepository;
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddEditBrandCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStringLocalizer<AddEditBrandCommandHandler> localizer,
        IBrandRepository brandRepository,
        IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
        _brandRepository = brandRepository;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(AddEditBrandCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == 0)
        {
            var brand = _mapper.Map<Brand>(command);
            _brandRepository.Add(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _cache.Remove(ApplicationConstants.Cache.GetAllBrandsCacheKey);
            return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Saved"]);
        }
        else
        {
            var brand = await _brandRepository.GetByIdAsync(command.Id, cancellationToken);
            if (brand == null)
            {
                return await Result<int>.FailAsync(_localizer["Brand Not Found!"]);
            }

            brand.Name = command.Name ?? brand.Name;
            brand.Tax = command.Tax == 0 ? brand.Tax : command.Tax;
            brand.Description = command.Description ?? brand.Description;
            
            _brandRepository.Update(brand);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _cache.Remove(ApplicationConstants.Cache.GetAllBrandsCacheKey);
            return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Updated"]);
        }
    }
}
