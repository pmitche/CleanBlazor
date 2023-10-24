using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Models.Chat;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public interface IChatManager : IManager
{
    Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

    Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
}
