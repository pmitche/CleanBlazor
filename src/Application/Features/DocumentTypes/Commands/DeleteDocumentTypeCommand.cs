using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentTypes.Commands;

[ExcludeFromCodeCoverage]
public sealed record DeleteDocumentTypeCommand(int Id) : ICommand<Result<int>>;

internal sealed class DeleteDocumentTypeCommandHandler : ICommandHandler<DeleteDocumentTypeCommand, Result<int>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IStringLocalizer<DeleteDocumentTypeCommandHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteDocumentTypeCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IDocumentRepository documentRepository,
        IStringLocalizer<DeleteDocumentTypeCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _documentRepository = documentRepository;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(DeleteDocumentTypeCommand command, CancellationToken cancellationToken)
    {
        var isDocumentTypeUsed = await _documentRepository.IsDocumentTypeUsed(command.Id);
        if (isDocumentTypeUsed)
        {
            return await Result<int>.FailAsync(_localizer["Deletion Not Allowed"]);
        }

        DocumentType documentType = await _unitOfWork.Repository<DocumentType>().GetByIdAsync(command.Id);
        if (documentType == null)
        {
            return await Result<int>.FailAsync(_localizer["Document Type Not Found!"]);
        }

        await _unitOfWork.Repository<DocumentType>().DeleteAsync(documentType);
        await _unitOfWork.CommitAndRemoveCache(cancellationToken,
            ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
        return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Deleted"]);
    }
}
