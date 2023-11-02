namespace CleanBlazor.Domain.Abstractions;

public interface IAuditableEntity : IEntity
{
    string CreatedBy { get; }
    DateTime CreatedOn { get; }
    string LastModifiedBy { get; }
    DateTime? LastModifiedOn { get; }
}
