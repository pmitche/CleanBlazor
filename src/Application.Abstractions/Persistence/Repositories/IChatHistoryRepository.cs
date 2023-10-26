using BlazorHero.CleanArchitecture.Domain.Entities.Communication;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Persistence.Repositories;

public interface IChatHistoryRepository : IRepository<ChatHistory<IChatUser>, long>
{

}
