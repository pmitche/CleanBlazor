namespace BlazorHero.CleanArchitecture.Domain.Abstractions;

public interface ISoftDeletableEntity
{
    string DeletedBy { get; protected set; }
    DateTimeOffset? DeletedOnUtc { get; protected set; }
    bool Deleted { get; protected set; }

    public void Restore()
    {
        DeletedBy = null;
        DeletedOnUtc = null;
        Deleted = false;
    }
}
