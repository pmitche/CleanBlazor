namespace BlazorHero.CleanArchitecture.Domain.Abstractions;

public interface ISoftDeletableEntity
{
    string DeletedBy { get; }
    DateTime? DeletedOn { get; }
    bool Deleted { get; }

    /// <summary>
    /// Restore the soft deletable entity.
    /// Should set Deleted to <c>false</c>, DeletedOn to <c>null</c> and DeletedBy to <c>null</c>.
    /// </summary>
    void Restore();
}
