using BlazorHero.CleanArchitecture.Domain.Abstractions;
using BlazorHero.CleanArchitecture.Domain.Primitives;

namespace BlazorHero.CleanArchitecture.Domain.Entities.Misc;

public class Document : AggregateRoot<int>, IAuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public string Url { get; set; }
    public int DocumentTypeId { get; set; }
    public virtual DocumentType DocumentType { get; set; }
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
}
