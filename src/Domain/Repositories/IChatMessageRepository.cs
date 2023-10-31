using BlazorHero.CleanArchitecture.Domain.Entities.Communication;

namespace BlazorHero.CleanArchitecture.Domain.Repositories;

public interface IChatMessageRepository : IRepository<ChatMessage<IChatUser>, long>
{

}
