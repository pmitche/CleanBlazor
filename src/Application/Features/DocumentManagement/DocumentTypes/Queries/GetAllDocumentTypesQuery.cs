using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAllDocumentTypesQuery : IQuery<Result<List<GetAllDocumentTypesResponse>>>;

internal sealed class GetAllDocumentTypesQueryHandler
    : IQueryHandler<GetAllDocumentTypesQuery, Result<List<GetAllDocumentTypesResponse>>>
{
    private readonly IAppCache _cache;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IMapper _mapper;

    public GetAllDocumentTypesQueryHandler(
        IDocumentTypeRepository documentTypeRepository,
        IMapper mapper,
        IAppCache cache)
    {
        _documentTypeRepository = documentTypeRepository;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<List<GetAllDocumentTypesResponse>>> Handle(
        GetAllDocumentTypesQuery request,
        CancellationToken cancellationToken)
    {
        var documentTypes = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDocumentTypesCacheKey,
                () => _documentTypeRepository.GetAllAsync(cancellationToken));
        var mappedDocumentTypes = _mapper.Map<List<GetAllDocumentTypesResponse>>(documentTypes);
        return mappedDocumentTypes;
    }
}
