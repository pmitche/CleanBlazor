namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

public interface ICurrentUserService
{
    string UserId { get; }
}
