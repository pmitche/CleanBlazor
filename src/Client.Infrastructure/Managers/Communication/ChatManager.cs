using System.Net.Http.Json;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Extensions;
using BlazorHero.CleanArchitecture.Client.Infrastructure.Routes;
using BlazorHero.CleanArchitecture.Contracts.Chat;
using BlazorHero.CleanArchitecture.Domain.Entities.Communication;
using BlazorHero.CleanArchitecture.Shared.Wrapper;

namespace BlazorHero.CleanArchitecture.Client.Infrastructure.Managers.Communication;

public class ChatManager : IChatManager
{
    private readonly HttpClient _httpClient;

    public ChatManager(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<Result<IEnumerable<ChatMessageResponse>>> GetChatHistoryAsync(string chatId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetChatHistory(chatId));
        Result<IEnumerable<ChatMessageResponse>> data = await response.ToResult<IEnumerable<ChatMessageResponse>>();
        return data;
    }

    public async Task<Result<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
    {
        HttpResponseMessage response = await _httpClient.GetAsync(ChatEndpoint.GetAvailableUsers);
        Result<IEnumerable<ChatUserResponse>> data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<Result> SaveMessageAsync(ChatMessage<IChatUser> chatMessage)
    {
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ChatEndpoint.SaveMessage, chatMessage);
        Result data = await response.ToResult();
        return data;
    }
}
