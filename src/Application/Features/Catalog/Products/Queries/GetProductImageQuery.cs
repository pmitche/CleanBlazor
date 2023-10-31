using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Products.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetProductImageQuery(int Id) : IQuery<Result<string>>;

internal sealed class GetProductImageQueryHandler : IQueryHandler<GetProductImageQuery, Result<string>>
{
    private readonly IProductRepository _productRepository;

    public GetProductImageQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<string>> Handle(GetProductImageQuery request, CancellationToken cancellationToken)
    {
        var data = await _productRepository.Entities
            .Where(p => p.Id == request.Id)
            .Select(a => a.ImageDataUrl)
            .FirstOrDefaultAsync(cancellationToken);

        return await Result<string>.SuccessAsync(data: data);
    }
}
