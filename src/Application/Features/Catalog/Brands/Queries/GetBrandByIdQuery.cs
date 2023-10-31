﻿using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Contracts.Catalog.Brands;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.Catalog.Brands.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetBrandByIdQuery(int Id) : IQuery<Result<GetBrandByIdResponse>>;

internal sealed class GetProductByIdQueryHandler : IQueryHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IBrandRepository brandRepository, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.GetByIdAsync(query.Id, cancellationToken);
        var mappedBrand = _mapper.Map<GetBrandByIdResponse>(brand);
        return mappedBrand;
    }
}
