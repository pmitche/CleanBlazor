using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Catalog;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using MediatR;

namespace BlazorHero.CleanArchitecture.Application.Features.Brands.Queries.GetAll;

public class GetAllBrandsQuery : IRequest<Result<List<GetAllBrandsResponse>>>
{
}

internal class GetAllBrandsCachedQueryHandler : IRequestHandler<GetAllBrandsQuery, Result<List<GetAllBrandsResponse>>>
{
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllBrandsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<List<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request,
        CancellationToken cancellationToken)
    {
        Task<List<Brand>> GetAllBrands() => _unitOfWork.Repository<Brand>().GetAllAsync();
        List<Brand> brandList =
            await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBrandsCacheKey, GetAllBrands);
        var mappedBrands = _mapper.Map<List<GetAllBrandsResponse>>(brandList);
        return await Result<List<GetAllBrandsResponse>>.SuccessAsync(mappedBrands);
    }
}
