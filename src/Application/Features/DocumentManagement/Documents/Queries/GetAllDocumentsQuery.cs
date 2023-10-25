using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Application.Extensions;
using BlazorHero.CleanArchitecture.Application.Specifications.Misc;
using BlazorHero.CleanArchitecture.Contracts.Documents;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.Documents.Queries;

[ExcludeFromCodeCoverage]
public sealed record GetAllDocumentsQuery(int PageNumber, int PageSize, string SearchString)
    : IQuery<PaginatedResult<GetAllDocumentsResponse>>;

internal sealed class GetAllDocumentsQueryHandler
    : IQueryHandler<GetAllDocumentsQuery, PaginatedResult<GetAllDocumentsResponse>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllDocumentsQueryHandler(IDocumentRepository documentRepository, ICurrentUserService currentUserService)
    {
        _documentRepository = documentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PaginatedResult<GetAllDocumentsResponse>> Handle(
        GetAllDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        Expression<Func<Document, GetAllDocumentsResponse>> expression = e => new GetAllDocumentsResponse
        {
            Id = e.Id,
            Title = e.Title,
            CreatedBy = e.CreatedBy,
            IsPublic = e.IsPublic,
            CreatedOn = e.CreatedOn,
            Description = e.Description,
            Url = e.Url,
            DocumentType = e.DocumentType.Name,
            DocumentTypeId = e.DocumentTypeId
        };
        DocumentFilterSpecification docSpec = new(request.SearchString, _currentUserService.UserId);
        PaginatedResult<GetAllDocumentsResponse> data = await _documentRepository.Entities
            .Specify(docSpec)
            .Select(expression)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
        return data;
    }
}
