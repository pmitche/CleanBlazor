namespace BlazorHero.CleanArchitecture.Domain.Contracts;

public abstract class Entity<TId> : IEntity<TId>
{
    public TId Id { get; set; }
}
