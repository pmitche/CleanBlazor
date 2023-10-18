﻿using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;

namespace BlazorHero.CleanArchitecture.Application.Features.Documents.Queries.GetById;

public class GetDocumentByIdQuery : IRequest<Result<GetDocumentByIdResponse>>
{
    public int Id { get; set; }
}

internal class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<GetDocumentByIdResponse>>
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
