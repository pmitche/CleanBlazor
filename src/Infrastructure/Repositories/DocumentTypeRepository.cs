using BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;
using BlazorHero.CleanArchitecture.Domain.Entities.Misc;
using BlazorHero.CleanArchitecture.Infrastructure.Contexts;

namespace BlazorHero.CleanArchitecture.Infrastructure.Repositories;

internal sealed class DocumentTypeRepository : GenericRepository<DocumentType, int>, IDocumentTypeRepository
{
    public DocumentTypeRepository(BlazorHeroContext dbContext) : base(dbContext)
    {
    }
}
