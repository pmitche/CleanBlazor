using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Commands;

[ExcludeFromCodeCoverage]
public sealed record DeleteBrandCommand(int Id) : ICommand<Result<int>>;

internal sealed class DeleteBrandCommandHandler : ICommandHandler<DeleteBrandCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteBrandCommandHandler> _localizer;
    private readonly IAppCache _cache;
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBrandCommandHandler(
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        IStringLocalizer<DeleteBrandCommandHandler> localizer,
        IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _localizer = localizer;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        var isBrandUsed = await _productRepository.IsBrandUsedAsync(command.Id, cancellationToken);
        if (isBrandUsed)
        {
            return Result.Fail<int>(_localizer["Deletion Not Allowed"]);
        }

        var brand = await _brandRepository.GetByIdAsync(command.Id, cancellationToken);
        if (brand == null)
        {
            return Result.Fail<int>(_localizer["Brand Not Found!"]);
        }

        _brandRepository.Remove(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(ApplicationConstants.Cache.GetAllBrandsCacheKey);
        return Result.Ok(brand.Id, _localizer["Brand Deleted"]);
    }
}
