using System.Diagnostics.CodeAnalysis;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.Documents.Commands;

[ExcludeFromCodeCoverage]
public sealed record DeleteDocumentCommand(int Id) : ICommand<Result<int>>;

internal sealed class DeleteDocumentCommandHandler : ICommandHandler<DeleteDocumentCommand, Result<int>>
{
    private readonly IStringLocalizer<DeleteDocumentCommandHandler> _localizer;
    private readonly IUnitOfWork<int> _unitOfWork;

    public DeleteDocumentCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<DeleteDocumentCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<Result<int>> Handle(DeleteDocumentCommand command, CancellationToken cancellationToken)
    {
        Document document = await _unitOfWork.Repository<Document>().GetByIdAsync(command.Id);
        if (document == null)
        {
            return await Result<int>.FailAsync(_localizer["Document Not Found!"]);
        }

        await _unitOfWork.Repository<Document>().DeleteAsync(document);
        await _unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
        return await Result<int>.SuccessAsync(document.Id, _localizer["Document Deleted"]);
    }
}
