using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAllBrandsQuery : IQuery<Result<List<GetAllBrandsResponse>>>;

internal sealed class GetAllBrandsCachedQueryHandler : IQueryHandler<GetAllBrandsQuery, Result<List<GetAllBrandsResponse>>>
{
    private readonly IAppCache _cache;
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public GetAllBrandsCachedQueryHandler(
        IMapper mapper,
        IAppCache cache,
        IBrandRepository brandRepository)
    {
        _mapper = mapper;
        _cache = cache;
        _brandRepository = brandRepository;
    }

    public async Task<Result<List<GetAllBrandsResponse>>> Handle(
        GetAllBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var brands = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBrandsCacheKey,
                () => _brandRepository.GetAllAsync(cancellationToken));
        var mappedBrands = _mapper.Map<List<GetAllBrandsResponse>>(brands);
        return await Result<List<GetAllBrandsResponse>>.SuccessAsync(mappedBrands);
    }
}
