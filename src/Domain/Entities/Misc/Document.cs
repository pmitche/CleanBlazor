using BlazorHero.CleanArchitecture.Domain.Contracts;

namespace BlazorHero.CleanArchitecture.Domain.Entities.Misc;

public class Document : AuditableEntity<int>
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public string Url { get; set; }
    public int DocumentTypeId { get; set; }
    public virtual DocumentType DocumentType { get; set; }
}
