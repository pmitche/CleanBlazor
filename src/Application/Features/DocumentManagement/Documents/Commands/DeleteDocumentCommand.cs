using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.Documents.Commands;

[ExcludeFromCodeCoverage]
public sealed record DeleteDocumentCommand(int Id) : ICommand<Result<int>>;

internal sealed class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteDocumentCommandHandler> _localizer;
    private readonly IDocumentRepository _documentRepository;
    private readonly IAppCache _cache;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDocumentCommandHandler(
        IUnitOfWork unitOfWork,
        IStringLocalizer<DeleteDocumentCommandHandler> localizer,
        IDocumentRepository documentRepository,
        IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _documentRepository = documentRepository;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
    {
        Document document = await _documentRepository.GetByIdAsync(command.Id, cancellationToken);
        if (document == null)
        {
            return await Result<int>.FailAsync(_localizer["Document Not Found!"]);
        }

        _documentRepository.Remove(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(ApplicationConstants.Cache.GetAllBrandsCacheKey);
        return await Result<int>.SuccessAsync(document.Id, _localizer["Document Deleted"]);
    }
}
