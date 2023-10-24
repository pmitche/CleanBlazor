using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetProductImage;

public record GetProductImageQuery(int Id) : IQuery<Result<string>>;

internal class GetProductImageQueryHandler : IQueryHandler<GetProductImageQuery, Result<string>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetProductImageQueryHandler(IUnitOfWork<int> unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Result<string>> Handle(GetProductImageQuery request, CancellationToken cancellationToken)
    {
        var data = await _unitOfWork.Repository<Product>().Entities.Where(p => p.Id == request.Id)
            .Select(a => a.ImageDataUrl).FirstOrDefaultAsync(cancellationToken);
        return await Result<string>.SuccessAsync(data: data);
    }
}
