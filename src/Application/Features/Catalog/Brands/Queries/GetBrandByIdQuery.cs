using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetBrandByIdQuery(int Id) : IQuery<Result<GetBrandByIdResponse>>;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        Brand brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(query.Id);
        var mappedBrand = _mapper.Map<GetBrandByIdResponse>(brand);
        return await Result<GetBrandByIdResponse>.SuccessAsync(mappedBrand);
    }
}
