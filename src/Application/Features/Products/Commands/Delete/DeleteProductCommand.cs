﻿using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Commands.Delete;

public class DeleteProductCommand : IRequest<Result<int>>
{
    public int Id { get; set; }
}

internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteProductCommandHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteProductCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<DeleteProductCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await _unitOfWork.Repository<Product>().GetByIdAsync(command.Id);
        if (product == null)
        {
            return await Result<int>.FailAsync(_localizer["Product Not Found!"]);
        }

        await _unitOfWork.Repository<Product>().DeleteAsync(product);
        await _unitOfWork.Commit(cancellationToken);
        return await Result<int>.SuccessAsync(product.Id, _localizer["Product Deleted"]);
    }
}
