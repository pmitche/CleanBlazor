using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetDocumentTypeByIdQuery(int Id) : IQuery<Result<GetDocumentTypeByIdResponse>>;

internal sealed class GetDocumentTypeByIdQueryHandler
    : IQueryHandler<GetDocumentTypeByIdQuery, Result<GetDocumentTypeByIdResponse>>
{
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IMapper _mapper;

    public GetDocumentTypeByIdQueryHandler(IDocumentTypeRepository documentTypeRepository, IMapper mapper)
    {
        _documentTypeRepository = documentTypeRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetDocumentTypeByIdResponse>> Handle(
        GetDocumentTypeByIdQuery query,
        CancellationToken cancellationToken)
    {
        var documentType = await _documentTypeRepository.GetByIdAsync(query.Id, cancellationToken);
        var mappedDocumentType = _mapper.Map<GetDocumentTypeByIdResponse>(documentType);
        return await Result<GetDocumentTypeByIdResponse>.SuccessAsync(mappedDocumentType);
    }
}
