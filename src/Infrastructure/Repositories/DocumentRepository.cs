using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class DocumentRepository : GenericRepository<Document, int>, IDocumentRepository
{
    public DocumentRepository(BlazorHeroContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> IsDocumentTypeUsedAsync(int documentTypeId) =>
        await Entities.AnyAsync(b => b.DocumentTypeId == documentTypeId);
}
