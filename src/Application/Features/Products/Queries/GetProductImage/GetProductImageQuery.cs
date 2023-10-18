using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Application.Features.Products.Queries.GetProductImage;

public class GetProductImageQuery : IRequest<Result<string>>
{
    public GetProductImageQuery(int productId) => Id = productId;

    public int Id { get; set; }
}

internal class GetProductImageQueryHandler : IRequestHandler<GetProductImageQuery, Result<string>>
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
