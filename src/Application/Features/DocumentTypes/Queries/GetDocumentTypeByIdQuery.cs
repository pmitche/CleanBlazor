using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetDocumentTypeByIdQuery(int Id) : IQuery<Result<GetDocumentTypeByIdResponse>>;

internal sealed class
    GetDocumentTypeByIdQueryHandler : IQueryHandler<GetDocumentTypeByIdQuery, Result<GetDocumentTypeByIdResponse>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetDocumentTypeByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<GetDocumentTypeByIdResponse>> Handle(
        GetDocumentTypeByIdQuery query,
        CancellationToken cancellationToken)
    {
        DocumentType documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(query.Id);
        var mappedDocumentType = _mapper.Map<GetDocumentTypeByIdResponse>(documentType);
        return await Result<GetDocumentTypeByIdResponse>.SuccessAsync(mappedDocumentType);
    }
}
