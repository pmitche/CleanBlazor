using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Domain.Repositories;
using BlazorHero.CleanArchitecture.Infrastructure.Data;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class DocumentTypeRepository : GenericRepository<DocumentType, int>, IDocumentTypeRepository
{
    public DocumentTypeRepository(BlazorHeroContext dbContext) : base(dbContext)
    {
    }
}
