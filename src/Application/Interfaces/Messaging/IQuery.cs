using MediatR;

namespace BlazorHero.CleanArchitecture.Application.Interfaces.Messaging;

/// <summary>
/// Represents the query interface.
/// </summary>
/// <typeparam name="TResponse">The query response type.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
