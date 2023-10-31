using BlazorHero.CleanArchitecture.Domain.Abstractions;
using BlazorHero.CleanArchitecture.Domain.Primitives;

namespace BlazorHero.CleanArchitecture.Domain.Entities.Catalog;

public class Product : AggregateRoot<int>, IAuditableEntity
{
    public string Name { get; set; }
    public string Barcode { get; set; }
    public string ImageDataUrl { get; set; }
    public string Description { get; set; }
    public decimal Rate { get; set; }
    public int BrandId { get; set; }
    public virtual Brand Brand { get; set; }
    public string CreatedBy { get; }
    public DateTime CreatedOn { get; }
    public string LastModifiedBy { get; }
    public DateTime? LastModifiedOn { get; }
}
