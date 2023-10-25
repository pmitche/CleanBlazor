namespace BlazorHero.CleanArchitecture.Domain.Contracts;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId>
{
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
