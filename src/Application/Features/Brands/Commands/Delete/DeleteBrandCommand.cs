using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Commands.Delete;

public class DeleteBrandCommand : IRequest<Result<int>>
{
    public int Id { get; set; }
}

internal class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteBrandCommandHandler> _localizer;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteBrandCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IProductRepository productRepository,
        IStringLocalizer<DeleteBrandCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        var isBrandUsed = await _productRepository.IsBrandUsed(command.Id);
        if (!isBrandUsed)
        {
            Brand brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(command.Id);
            if (brand != null)
            {
                await _unitOfWork.Repository<Brand>().DeleteAsync(brand);
                await _unitOfWork.CommitAndRemoveCache(cancellationToken,
                    ApplicationConstants.Cache.GetAllBrandsCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, _localizer["Brand Deleted"]);
            }

            return await Result<int>.FailAsync(_localizer["Brand Not Found!"]);
        }

        return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
    }
}
