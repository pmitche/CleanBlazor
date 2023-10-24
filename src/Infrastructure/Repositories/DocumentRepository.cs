using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly IRepositoryAsync<Document, int> _repository;

    public DocumentRepository(IRepositoryAsync<Document, int> repository) => _repository = repository;

    public async Task<bool> IsDocumentTypeUsed(int documentTypeId) =>
        await _repository.Entities.AnyAsync(b => b.DocumentTypeId == documentTypeId);

    public async Task<bool> IsDocumentExtendedAttributeUsed(int documentExtendedAttributeId) =>
        await _repository.Entities.AnyAsync(b => b.ExtendedAttributes.Any(x => x.Id == documentExtendedAttributeId));
}
