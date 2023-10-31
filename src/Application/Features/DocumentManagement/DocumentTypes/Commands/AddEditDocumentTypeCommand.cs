using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using BlazorHero.CleanArchitecture.Application.Abstractions.Messaging;
using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Shared.Constants.Application;
using BlazorHero.CleanArchitecture.Shared.Wrapper;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorHero.CleanArchitecture.Application.Features.DocumentManagement.DocumentTypes.Commands;

[ExcludeFromCodeCoverage]
public sealed class AddEditDocumentTypeCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    public AddEditDocumentTypeCommand(int id, string name, string description)
    {
        Id = id;
        Name = name;
        Description = description;
    }
}

internal sealed class AddEditDocumentTypeCommandHandler : ICommandHandler<AddEditDocumentTypeCommand, Result<int>>
{
    private readonly IStringLocalizer<AddEditDocumentTypeCommandHandler> _localizer;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IAppCache _cache;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public AddEditDocumentTypeCommandHandler(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IStringLocalizer<AddEditDocumentTypeCommandHandler> localizer,
        IDocumentTypeRepository documentTypeRepository,
        IAppCache cache)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _localizer = localizer;
        _documentTypeRepository = documentTypeRepository;
        _cache = cache;
    }

    public async Task<Result<int>> Handle(AddEditDocumentTypeCommand command, CancellationToken cancellationToken)
    {
        if (await _documentTypeRepository.Entities.Where(p => p.Id != command.Id)
                .AnyAsync(p => p.Name == command.Name, cancellationToken))
        {
            return await Result<int>.FailAsync(_localizer["Document type with this name already exists."]);
        }

        if (command.Id == 0)
        {
            var documentType = _mapper.Map<DocumentType>(command);
            _documentTypeRepository.Add(documentType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _cache.Remove(ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
            return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Saved"]);
        }
        else
        {
            var documentType = await _documentTypeRepository.GetByIdAsync(command.Id, cancellationToken);
            if (documentType == null)
            {
                return await Result<int>.FailAsync(_localizer["Document Type Not Found!"]);
            }

            documentType.Name = command.Name ?? documentType.Name;
            documentType.Description = command.Description ?? documentType.Description;
            _documentTypeRepository.Update(documentType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _cache.Remove(ApplicationConstants.Cache.GetAllDocumentTypesCacheKey);
            return await Result<int>.SuccessAsync(documentType.Id, _localizer["Document Type Updated"]);
        }
    }
}
