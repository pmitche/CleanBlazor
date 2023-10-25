using BlazorHero.CleanArchitecture.Domain.Entities.Misc;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;

public interface IDocumentRepository : IRepository<Document, int>
{
    Task<bool> IsDocumentTypeUsedAsync(int documentTypeId);
}
