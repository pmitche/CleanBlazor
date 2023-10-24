using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.Documents.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetDocumentByIdQuery(int Id) : IQuery<Result<GetDocumentByIdResponse>>;

internal sealed class GetDocumentByIdQueryHandler : IQueryHandler<GetDocumentByIdQuery, Result<GetDocumentByIdResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetDocumentByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetDocumentByIdResponse>> Handle(
        GetDocumentByIdQuery query,
        CancellationToken cancellationToken)
    {
        Document document = await _unitOfWork.Repository<Document>().GetByIdAsync(query.Id);
        var mappedDocument = _mapper.Map<GetDocumentByIdResponse>(document);
        return await Result<GetDocumentByIdResponse>.SuccessAsync(mappedDocument);
    }
}
