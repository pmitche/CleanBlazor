using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAllDocumentTypesQuery : IQuery<Result<List<GetAllDocumentTypesResponse>>>;

internal sealed class
    GetAllDocumentTypesQueryHandler : IQueryHandler<GetAllDocumentTypesQuery,
        Result<List<GetAllDocumentTypesResponse>>>
{
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllDocumentTypesQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<Result<List<GetAllDocumentTypesResponse>>> Handle(
        GetAllDocumentTypesQuery request,
        CancellationToken cancellationToken)
    {
        Task<List<DocumentType>> GetAllDocumentTypes() => _unitOfWork.Repository<DocumentType>().GetAllAsync();
        List<DocumentType> documentTypeList =
            await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllDocumentTypesCacheKey, GetAllDocumentTypes);
        var mappedDocumentTypes =
            _mapper.Map<List<GetAllDocumentTypesResponse>>(documentTypeList);
        return await Result<List<GetAllDocumentTypesResponse>>.SuccessAsync(mappedDocumentTypes);
    }
}
