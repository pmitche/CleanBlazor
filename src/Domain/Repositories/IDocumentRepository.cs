using BlazorHero.CleanArchitecture.Domain.Entities.Misc;

namespace BlazorHero.CleanArchitecture.Domain.Repositories;

public interface IDocumentRepository : IRepository<Document, int>
{
    Task<bool> IsDocumentTypeUsedAsync(int documentTypeId);
}
