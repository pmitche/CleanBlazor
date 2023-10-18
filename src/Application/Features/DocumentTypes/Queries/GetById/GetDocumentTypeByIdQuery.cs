﻿using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Interfaces.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using MediatR;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Queries.GetById;

public class GetDocumentTypeByIdQuery : IRequest<Result<GetDocumentTypeByIdResponse>>
{
    public int Id { get; set; }
}

internal class
    GetDocumentTypeByIdQueryHandler : IRequestHandler<GetDocumentTypeByIdQuery, Result<GetDocumentTypeByIdResponse>>
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
