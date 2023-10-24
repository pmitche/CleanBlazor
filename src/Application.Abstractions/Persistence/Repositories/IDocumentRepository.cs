namespace BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;

public interface IDocumentRepository
{
    Task<bool> IsDocumentTypeUsed(int documentTypeId);

    Task<bool> IsDocumentExtendedAttributeUsed(int documentExtendedAttributeId);
}
