using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly IRepositoryAsync<DocumentType, int> _repository;

    public DocumentTypeRepository(IRepositoryAsync<DocumentType, int> repository) => _repository = repository;
}
