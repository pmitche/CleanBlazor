using BlazorHero.CleanArchitecture.Application.Interfaces.Chat;
using BlazorHero.CleanArchitecture.Application.Models.Chat;
using BlazorHero.CleanArchitecture.Application.Responses.Chat;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public interface IChatManager : IManager
{
    Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

    Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId);
}
