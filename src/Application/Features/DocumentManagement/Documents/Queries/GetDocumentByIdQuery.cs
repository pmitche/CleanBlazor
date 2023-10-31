using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.Documents.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetDocumentByIdQuery(int Id) : IQuery<Result<GetDocumentByIdResponse>>;

internal sealed class GetDocumentByIdQueryHandler : IQueryHandler<GetDocumentByIdQuery, Result<GetDocumentByIdResponse>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GetDocumentByIdQueryHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetDocumentByIdResponse>> Handle(
        GetDocumentByIdQuery query,
        CancellationToken cancellationToken)
    {
        Document document = await _documentRepository.GetByIdAsync(query.Id, cancellationToken);
        var mappedDocument = _mapper.Map<GetDocumentByIdResponse>(document);
        return mappedDocument;
    }
}
