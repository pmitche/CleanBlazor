using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetById;

public record GetBrandByIdQuery(int Id) : IRequest<Result<GetBrandByIdResponse>>;

internal class GetProductByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
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
