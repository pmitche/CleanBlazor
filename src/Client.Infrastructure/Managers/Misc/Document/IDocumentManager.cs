﻿using BlazorHero.CleanArchitecture.Application.Features.Documents.Commands;
using BlazorHero.CleanArchitecture.Application.Features.Documents.Queries;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Misc.Document;

public interface IDocumentManager : IManager
{
    Task<PaginatedResult<GetAllDocumentsResponse>> GetAllAsync(GetAllPagedDocumentsRequest request);

    Task<IResult<GetDocumentByIdResponse>> GetByIdAsync(GetDocumentByIdQuery request);

    Task<IResult<int>> SaveAsync(AddEditDocumentCommand request);

    Task<IResult<int>> DeleteAsync(int id);
}
