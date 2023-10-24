using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Contracts.Chat;
using BlazorHero.CleanArchitecture.Shared.Models.Chat;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Application.Abstractions.Infrastructure.Services;

public interface IChatService
{
    Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(string userId);

    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

    Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId);
}
