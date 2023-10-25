using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Commands;

[ExcludeFromCodeCoverage]
public sealed record DeleteProductCommand(int Id) : ICommand<Result<int>>;

internal sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteProductCommandHandler> _localizer;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        IStringLocalizer<DeleteProductCommandHandler> localizer,
        IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _productRepository = productRepository;
    }

    public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (product == null)
        {
            return await Result<int>.FailAsync(_localizer["Product Not Found!"]);
        }

        _productRepository.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(product.Id, _localizer["Product Deleted"]);
    }
}
