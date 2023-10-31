using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public interface IChatManager : IManager
{
    Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

    Task<Result> SaveMessageAsync(ChatMessage<IChatUser> chatMessage);

    Task<Result<IEnumerable<ChatMessageResponse>>> GetChatHistoryAsync(string chatId);
}
